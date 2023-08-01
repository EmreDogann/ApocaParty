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
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent), typeof(DisplayAgentPath))]
    [RequireComponent(typeof(PlateMouseInteraction))]
    public class MinionAI : MonoBehaviour, IRequestOwner, IWaiter
    {
        public MinionStateMachine StateMachine;
        public NavMeshAgent NavMeshAgent { get; private set; }

        [Separator("General")]
        public DisplayAgentPath pathDisplayer;
        [SerializeField] private Transform holderTransform;
        public bool enableWandering;
        [SerializeField] private bool _isAIActive;

        [Separator("UI")]
        public SpriteRenderer image;
        [SerializeField] private TextMeshProUGUI aiState;
        public ProgressBar progressBar;

        [Separator("Other Data")]
        public MinionActorSO actorData;

        public Camera MainCamera { get; private set; }
        [HideInInspector] public Request currentRequest;
        [HideInInspector] public PlateMouseInteraction plateInteraction;

        private MinionIdleState _minionIdleState;
        private MinionMovingState _minionMovingState;
        private MinionWorkingState _minionWorkingState;
        private CharacterBlackboard _blackboard;

        [HideInInspector] public IConsumable HoldingConsumable;
        [HideInInspector] public IConsumable TargetConsumable;
        [HideInInspector] public readonly int WaiterID = Guid.NewGuid().GetHashCode();

        private bool _shouldWander;
        private const float DistanceThreshold = 0.1f;
        private const float WanderWaitTime = 3.0f;
        private float _currentWanderTime;
        private const float SearchRadius = 3.0f;

        private void Start()
        {
            MainCamera = Camera.main;
            _blackboard = GetComponent<CharacterBlackboard>();
            pathDisplayer = GetComponent<DisplayAgentPath>();
            plateInteraction = GetComponent<PlateMouseInteraction>();

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
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f || !_isAIActive)
            {
                return;
            }

            StateMachine.UpdateState();
            aiState.text = StateMachine.GetCurrentState().GetID().ToString();

            _blackboard.IsMoving = NavMeshAgent.hasPath;

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

        private Vector3 RandomNavmeshLocation(Vector3 position, float radius)
        {
            Vector3 finalPosition = position;
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = ClampMagnitude(Random.insideUnitCircle * radius, Mathf.Infinity, 2.0f);
                randomDirection += position;

                if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit, NavMeshAgent.areaMask))
                {
                    finalPosition = raycastHit.position;
                    break;
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
            if (TargetConsumable != null)
            {
                if (TargetConsumable == other.GetComponent<IConsumable>())
                {
                    HoldingConsumable = TargetConsumable;
                    HoldingConsumable.Claim();
                    TargetConsumable = null;
                }
            }

            IWaiterTarget waiterTarget = other.GetComponent<IWaiterTarget>();
            if (waiterTarget != null && waiterTarget.GetWaiterID() == WaiterID)
            {
                waiterTarget.WaiterInteracted(this);
                HoldingConsumable = null;
            }
        }
    }
}