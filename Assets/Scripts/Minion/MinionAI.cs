using System;
using Actors;
using Consumable;
using GuestRequests;
using Interactions;
using Minion.States;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Minion
{
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent), typeof(DisplayAgentPath))]
    public class MinionAI : MonoBehaviour, IRequestOwner, IWaiter
    {
        public MinionStateMachine stateMachine;
        public NavMeshAgent navMeshAgent { get; private set; }

        public DisplayAgentPath pathDisplayer;

        public SpriteRenderer image;
        [SerializeField] private Transform holderTransform;
        [SerializeField] private TextMeshProUGUI AIState;

        public Camera _mainCamera { get; private set; }
        [HideInInspector] public Request currentRequest;
        public MinionActorSO actorData;

        private MinionIdleState _minionIdleState;
        private MinionMovingState _minionMovingState;
        private MinionWorkingState _minionWorkingState;
        private CharacterBlackboard _blackboard;

        private IConsumable _holdingConsumable;
        private IConsumable _targetConsumable;
        private readonly int _waiterID = Guid.NewGuid().GetHashCode();

        private bool _shouldWander;
        private const float DistanceThreshold = 0.1f;
        private const float WanderWaitTime = 3.0f;
        private float _currentWanderTime;
        private const float SearchRadius = 3.0f;

        private void Start()
        {
            _mainCamera = Camera.main;
            _blackboard = GetComponent<CharacterBlackboard>();
            pathDisplayer = GetComponent<DisplayAgentPath>();

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            stateMachine = new MinionStateMachine();
            _minionIdleState = new MinionIdleState(this, stateMachine);
            _minionMovingState = new MinionMovingState(this, stateMachine);
            _minionWorkingState = new MinionWorkingState(this, stateMachine);

            stateMachine.RegisterState(_minionIdleState);
            stateMachine.RegisterState(_minionMovingState);
            stateMachine.RegisterState(_minionWorkingState);

            // Initial state
            stateMachine.ChangeState(MinionStateID.Idle);
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            stateMachine.UpdateState();
            AIState.text = stateMachine.GetCurrentState().GetID().ToString();

            _blackboard.IsMoving = navMeshAgent.hasPath;

            if (_shouldWander)
            {
                if (Vector3.SqrMagnitude(transform.position - navMeshAgent.destination) <
                    DistanceThreshold * DistanceThreshold)
                {
                    _currentWanderTime += Time.deltaTime;
                }

                if (_currentWanderTime >= WanderWaitTime)
                {
                    _currentWanderTime = 0.0f;
                    navMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius));
                }
            }
        }

        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        public void SetDestinationAndDisplayPath(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
            pathDisplayer.DisplayPath();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Transform GetHoldingPosition()
        {
            return holderTransform;
        }

        public void OwnerRemoved()
        {
            currentRequest = null;
            stateMachine.ChangeState(MinionStateID.Idle);
        }

        public void OnInteract(InteractableBase interactableBase)
        {
            if (stateMachine.GetCurrentState().GetID() == MinionStateID.Idle)
            {
                ((MinionIdleState)stateMachine.GetCurrentState()).OnInteraction(interactableBase);
            }
        }

        public void SetWandering(bool isWandering)
        {
            _shouldWander = isWandering;
            if (!isWandering)
            {
                return;
            }

            navMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius));
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

                if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit, navMeshAgent.areaMask))
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
            throw new NotImplementedException();
        }
    }
}