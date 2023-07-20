using System;
using Actors;
using Guest.States;
using GuestRequests;
using Interactions.Interactables;
using MyBox;
using Needs;
using PartyEvents;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Guest
{
    public enum GuestType
    {
        Famine,
        War,
        Pestilence,
        Death,
        Henchmen
    }

    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent), typeof(GuestInteractable))]
    [RequireComponent(typeof(NeedSystem))]
    public class GuestAI : MonoBehaviour, IRequestOwner
    {
        [SerializeField] private GuestType _guestType;

        public GuestStateMachine stateMachine;
        public NavMeshAgent navMeshAgent { get; private set; }
        public GuestInteractable InteractableState { get; private set; }

        [Separator("Guest Data")]
        public SpriteRenderer image;
        [SerializeField] private Transform holderTransform;
        public ActorSO actorData;

        [Separator("Moods")]
        [SerializeField] private MoodType startingMood;

        [Separator("AI Behaviour")]
        [Range(0.0f, 1.0f)] public float chanceToWanderWhenHappy = 0.2f;
        public float wanderCheckFrequency = 5.0f;

        [Separator("Debugging")]
        [SerializeField] private TextMeshProUGUI AIState;

        public Camera _mainCamera { get; private set; }
        [HideInInspector] public Request currentRequest;

        private GuestIdleState _guestIdleState;
        private GuestMovingState _guestMovingState;
        private GuestWanderState _guestWanderState;
        private GuestConsumeState _guestConsumeState;
        private CharacterBlackboard _blackboard;
        public NeedSystem needSystem;

        // private bool _shouldWander;
        // private const float DistanceThreshold = 0.1f;
        // private const float WanderWaitTime = 3.0f;
        // private float _currentWanderTime;
        // private const float SearchRadius = 3.0f;

        private void Awake()
        {
            needSystem = GetComponent<NeedSystem>();
            needSystem.ChangeMood(startingMood);
        }

        private void OnEnable()
        {
            PartyEvent.OnPartyEvent += OnPartyEvent;
            needSystem.OnNewNeed += OnNewNeed;
        }

        private void OnDisable()
        {
            PartyEvent.OnPartyEvent -= OnPartyEvent;
            needSystem.OnNewNeed -= OnNewNeed;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            _blackboard = GetComponent<CharacterBlackboard>();
            InteractableState = GetComponent<GuestInteractable>();

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            stateMachine = new GuestStateMachine();
            _guestIdleState = new GuestIdleState(this, stateMachine);
            _guestMovingState = new GuestMovingState(this, stateMachine);
            _guestWanderState = new GuestWanderState(this, stateMachine);
            _guestConsumeState = new GuestConsumeState(this, stateMachine);

            stateMachine.RegisterState(_guestIdleState);
            stateMachine.RegisterState(_guestMovingState);
            stateMachine.RegisterState(_guestWanderState);
            stateMachine.RegisterState(_guestConsumeState);

            // Initial state
            stateMachine.ChangeState(GuestStateID.Idle);
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

            // if (_shouldWander)
            // {
            //     if (Vector3.SqrMagnitude(transform.position - navMeshAgent.destination) <
            //         DistanceThreshold * DistanceThreshold)
            //     {
            //         _currentWanderTime += Time.deltaTime;
            //     }
            //
            //     if (_currentWanderTime >= WanderWaitTime)
            //     {
            //         _currentWanderTime = 0.0f;
            //         navMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius));
            //     }
            // }
        }

        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Transform GetHoldingPosition()
        {
            return holderTransform;
        }
        
        private void OnNewNeed(INeed need)
        {
            switch (need.GetNeedType())
            {
                case NeedType.Drink:
                    
            }
        }

        private void OnPartyEvent(PartyEventData eventData)
        {
            switch (eventData.eventType)
            {
                case PartyEventType.FamineAtDrinks:
                    needSystem.ChangeMood(_guestType == GuestType.Famine
                        ? Mathf.Abs(eventData.moodCost)
                        : eventData.moodCost);
                    break;
                case PartyEventType.MusicPlaying:
                    needSystem.TryFulfillNeed(NeedType.Music);
                    break;
                case PartyEventType.MusicMachineBreaks:
                    break;
                case PartyEventType.FoodBurning:
                    break;
                case PartyEventType.PowerOutage:
                    break;
                case PartyEventType.BuntingFall:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // public void SetWandering(bool isWandering)
        // {
        //     _shouldWander = isWandering;
        //     if (!isWandering)
        //     {
        //         return;
        //     }
        //
        //     navMeshAgent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius));
        //     _currentWanderTime = 0.0f;
        // }
        //
        // public bool IsWandering()
        // {
        //     return _shouldWander;
        // }
        //
        // private Vector3 RandomNavmeshLocation(Vector3 position, float radius)
        // {
        //     Vector3 finalPosition = position;
        //     for (int i = 0; i < 30; i++)
        //     {
        //         Vector3 randomDirection = ClampMagnitude(Random.insideUnitCircle * radius, Mathf.Infinity, 2.0f);
        //         randomDirection += position;
        //
        //         if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit,
        //                 NavMesh.GetAreaFromName("AvoidWander")))
        //         {
        //             finalPosition = raycastHit.position;
        //             break;
        //         }
        //     }
        //
        //     finalPosition.z = 0;
        //     return finalPosition;
        // }
        //
        // private Vector3 ClampMagnitude(Vector3 v, float max, float min)
        // {
        //     double sm = v.sqrMagnitude;
        //     if (sm > max * (double)max)
        //     {
        //         return v.normalized * max;
        //     }
        //
        //     if (sm < min * (double)min)
        //     {
        //         return v.normalized * min;
        //     }
        //
        //     return v;
        // }
    }
}