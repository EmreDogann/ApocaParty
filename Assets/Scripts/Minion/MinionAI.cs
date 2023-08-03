using System;
using Actors;
using Consumable;
using GuestRequests;
using Interactions;
using Minion.States;
using MyBox;
using Player;
using TMPro;
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

        [Separator("UI")]
        public SpriteRenderer image;
        [SerializeField] private TextMeshProUGUI aiState;
        public ProgressBar progressBar;

        [Separator("Other Data")]
        public MinionActorSO actorData;

        public Camera MainCamera { get; private set; }
        [HideInInspector] public Request currentRequest;

        private MinionIdleState _minionIdleState;
        private MinionMovingState _minionMovingState;
        private MinionWorkingState _minionWorkingState;

        [HideInInspector] public IConsumable HoldingConsumable;
        [HideInInspector] public IConsumable TargetConsumable;
        [HideInInspector] public readonly int WaiterID = Guid.NewGuid().GetHashCode();

        private bool _shouldWander;
        private readonly float DistanceThreshold = 0.1f;
        private readonly float WanderWaitTime = 3.0f;
        private float _currentWanderTime;
        internal float SearchRadius = 3.0f;

        private bool _isAIActive;
        private int _characterLayerMask;

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

            StateMachine.RegisterState(_minionIdleState);
            StateMachine.RegisterState(_minionMovingState);
            StateMachine.RegisterState(_minionWorkingState);

            // Initial state
            StateMachine.ChangeState(MinionStateID.Idle);

            if (activateAIOnAwake)
            {
                SetActiveMinionAI(true);
            }

            _characterLayerMask = LayerMask.NameToLayer("Character");
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f || !_isAIActive)
            {
                return;
            }

            StateMachine.UpdateState();
            aiState.text = StateMachine.GetCurrentState().GetID().ToString();

            if (_shouldWander)
            {
                if (Vector3.SqrMagnitude(transform.position - NavMeshAgent.destination) <
                    DistanceThreshold * DistanceThreshold)
                {
                    _currentWanderTime += Time.deltaTime;
                }

                if (_currentWanderTime >= WanderWaitTime)
                {
                    _currentWanderTime = 0.0f;
                    NavMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius));
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
            StateMachine.ChangeState(MinionStateID.Idle);
        }

        public OwnerType GetOwnerType()
        {
            return OwnerType.Minion;
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

            NavMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius));
            _currentWanderTime = 0.0f;
        }

        public bool IsWandering()
        {
            return _shouldWander;
        }

        internal Vector3 RandomNavmeshLocation(Vector3 position, float radius)
        {
            Vector3 finalPosition = position;
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = ClampMagnitude(Random.insideUnitCircle * radius, Mathf.Infinity, 2.0f);
                randomDirection += position;

                if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit, NavMeshAgent.areaMask))
                {
                    if (Physics2D.OverlapCircle(raycastHit.position, 1.0f, 1 << _characterLayerMask) == null)
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            IWaiterTarget waiterTarget = other.GetComponent<IWaiterTarget>();
            if (waiterTarget != null && waiterTarget.IsAssignedWaiter() && waiterTarget.GetWaiterID() == WaiterID)
            {
                waiterTarget.WaiterInteracted(this);
                if (!waiterTarget.HasUnknownRequest())
                {
                    NavMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius * 0.2f));
                    pathDisplayer.HidePath();
                    HoldingConsumable = null;
                }
            }
        }
    }
}