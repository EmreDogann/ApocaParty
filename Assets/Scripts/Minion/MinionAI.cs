using System;
using Actors;
using Audio;
using Consumable;
using GuestRequests;
using Interactions;
using Minion.States;
using MyBox;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Minion
{
    [RequireComponent(typeof(NavMeshAgent), typeof(DisplayAgentPath))]
    public class MinionAI : MonoBehaviour, IRequestOwner, IWaiter
    {
        public MinionStateMachine StateMachine;
        public NavMeshAgent NavMeshAgent { get; private set; }

        [Separator("General")]
        public DisplayAgentPath pathDisplayer;
        [SerializeField] private Transform holderTransform;
        public bool enableWandering;
        [SerializeField] private bool activateAIOnAwake;
        public AudioSO errorSound;

        [Separator("Spill Settings")]
        [Range(0.0f, 1.0f)] public float chanceToSpillFood;
        [Range(0.0f, 1.0f)] public float chanceToSlip;
        public float spillFoodCheckFrequency;
        public AudioSO slipAudio;
        public float cleanupTime;
        public int maxCleanupAmount = 5;

        [Separator("UI")]
        public SpriteRenderer image;
        public ProgressBar progressBar;

        [Separator("Other Data")]
        public MinionActorSO actorData;

        public Camera MainCamera { get; private set; }
        [HideInInspector] public Request currentRequest;

        private MinionIdleState _minionIdleState;
        private MinionMovingState _minionMovingState;
        private MinionWorkingState _minionWorkingState;
        private MinionSlipState _minionSlipState;
        private MinionCleanupState _minionCleanupState;

        [HideInInspector] public IConsumable HoldingConsumable;
        [HideInInspector] public IConsumable TargetConsumable;
        [HideInInspector] public readonly int WaiterID = Guid.NewGuid().GetHashCode();
        [HideInInspector] public IWaiterTarget WaiterTarget;

        private bool _shouldWander;
        private readonly float _distanceThreshold = 0.1f;
        private readonly float _wanderWaitTime = 3.0f;
        private float _currentWanderTime;
        internal float SearchRadius = 3.0f;

        private bool _isAIActive;
        private int _characterLayer;
        internal int SpillLayer;

        private float _spillFoodTimer;

        private void Awake()
        {
            MainCamera = Camera.main;
            pathDisplayer = GetComponent<DisplayAgentPath>();

            NavMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshAgent.updateRotation = false;
            NavMeshAgent.updateUpAxis = false;

            StateMachine = new MinionStateMachine();
            _minionIdleState = new MinionIdleState(this, StateMachine);
            _minionMovingState = new MinionMovingState(this, StateMachine);
            _minionWorkingState = new MinionWorkingState(this, StateMachine);
            _minionSlipState = new MinionSlipState(this, StateMachine);
            _minionCleanupState = new MinionCleanupState(this, StateMachine);

            StateMachine.RegisterState(_minionIdleState);
            StateMachine.RegisterState(_minionMovingState);
            StateMachine.RegisterState(_minionWorkingState);
            StateMachine.RegisterState(_minionSlipState);
            StateMachine.RegisterState(_minionCleanupState);

            // Initial state
            StateMachine.ChangeState(MinionStateID.Idle);

            if (activateAIOnAwake)
            {
                SetActiveMinionAI(true);
            }

            _characterLayer = LayerMask.NameToLayer("Character");
            SpillLayer = LayerMask.NameToLayer("Spillable");
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f || !_isAIActive)
            {
                return;
            }

            StateMachine.UpdateState();

            if (_shouldWander)
            {
                if (Vector3.SqrMagnitude(transform.position - NavMeshAgent.destination) <
                    _distanceThreshold * _distanceThreshold)
                {
                    _currentWanderTime += Time.deltaTime;
                }

                if (_currentWanderTime >= _wanderWaitTime)
                {
                    _currentWanderTime = 0.0f;
                    NavMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius,
                        NavMeshAgent.areaMask));
                }
            }
        }

        public void SetActiveMinionAI(bool isActive)
        {
            _isAIActive = isActive;
        }

        public void SetActiveWandering(bool isActive)
        {
            enableWandering = isActive;
        }

        public void SetDestination(Vector3 target)
        {
            NavMeshAgent.SetDestination(target);
        }

        public void SetDestinationAndDisplayPath(Vector3 target)
        {
            NavMeshAgent.SetDestination(target);
            pathDisplayer.DisplayPath();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Transform GetHoldingTransform()
        {
            return holderTransform;
        }

        public void OwnerRemoved()
        {
            currentRequest = null;
            NavMeshAgent.ResetPath();
            StateMachine.ChangeState(MinionStateID.Idle);
        }

        public CharacterType GetOwnerType()
        {
            return CharacterType.Minion;
        }

        public CharacterType GetWaiterType()
        {
            return CharacterType.Minion;
        }

        public void OnInteract(InteractableBase interactableBase)
        {
            if (StateMachine.GetCurrentState().GetID() == MinionStateID.Idle)
            {
                ((MinionIdleState)StateMachine.GetCurrentState()).OnInteraction(interactableBase);
            }
        }

        public void SetWandering(bool isWandering)
        {
            _shouldWander = isWandering;
            if (!isWandering)
            {
                return;
            }

            NavMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius, NavMeshAgent.areaMask));
            _currentWanderTime = 0.0f;
        }

        public bool IsWandering()
        {
            return _shouldWander;
        }

        internal Vector3 RandomNavmeshLocation(Vector3 position, float radius, int areaMask)
        {
            Vector3 finalPosition = position;
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = ClampMagnitude(Random.insideUnitCircle * radius, Mathf.Infinity, 2.0f);
                randomDirection += position;

                if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit, areaMask))
                {
                    if (Physics2D.OverlapCircle(raycastHit.position, 1.0f, 1 << _characterLayer) == null)
                    {
                        finalPosition = raycastHit.position;
                        break;
                    }
                }
            }

            finalPosition.z = 0;
            return finalPosition;
        }

        private Vector3 ClampMagnitude(Vector3 v, float max, float min)
        {
            double sm = v.sqrMagnitude;
            if (sm > max * (double)max)
            {
                return v.normalized * max;
            }

            if (sm < min * (double)min)
            {
                return v.normalized * min;
            }

            return v;
        }

        public IConsumable GetConsumable()
        {
            return HoldingConsumable;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer != SpillLayer ||
                StateMachine.GetCurrentState().GetID() != MinionStateID.Moving ||
                TargetConsumable != null && TargetConsumable.IsSpilled() ||
                HoldingConsumable != null && other.gameObject == HoldingConsumable.GetTransform().gameObject)
            {
                return;
            }

            _spillFoodTimer += Time.deltaTime;
            if (_spillFoodTimer < spillFoodCheckFrequency)
            {
                return;
            }

            _spillFoodTimer = 0.0f;
            if (HoldingConsumable != null)
            {
                if (Random.Range(0.0f, 1.0f) < chanceToSpillFood)
                {
                    HoldingConsumable.Spill();
                    HoldingConsumable = null;
                    StateMachine.ChangeState(MinionStateID.Slip);
                }
            }
            else if (Random.Range(0.0f, 1.0f) < chanceToSlip)
            {
                StateMachine.ChangeState(MinionStateID.Slip);
            }
        }
    }
}