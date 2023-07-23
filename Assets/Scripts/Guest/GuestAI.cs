using System;
using Actors;
using Consumable;
using DiningTable;
using Guest.States;
using GuestRequests;
using Interactions.Interactables;
using MyBox;
using Needs;
using PartyEvents;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    public class GuestAI : MonoBehaviour, IGuestRequestOwner
    {
        [SerializeField] private GuestType _guestType;
        public GuestType GuestType
        {
            get => _guestType;
            private set => _guestType = value;
        }

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
        [ConditionalField(nameof(_guestType), false, GuestType.Famine)] [Range(0.0f, 1.0f)]
        [SerializeField] private float walkToDrinksChance;

        [Range(0.0f, 1.0f)] public float wanderWhenHappyChance = 0.2f;
        public float wanderCheckFrequency = 5.0f;

        [Separator("Debugging")]
        [SerializeField] private TextMeshProUGUI AIState;

        public NeedSystem needSystem { get; private set; }
        public Camera _mainCamera { get; private set; }

        private GuestIdleState _guestIdleState;
        private GuestMovingState _guestMovingState;
        private GuestWanderState _guestWanderState;
        private GuestConsumeState _guestConsumeState;
        private GuestGetConsumableState _guestGetConsumableState;
        private GuestMoveToSeatState _guestMoveToSeatState;
        private CharacterBlackboard _blackboard;

        public IConsumable CurrentConsumable;
        [field: SerializeReference] public TableSeat AssignedTableSeat { get; private set; }

        private void OnEnable()
        {
            PartyEvent.OnPartyEvent += OnPartyEvent;
            needSystem.OnNewNeed += OnNewNeed;

            AssignedTableSeat.OnFoodArrival += OnFoodArrival;
            InteractableState.OnPlayerInteract += OnPlayerInteract;
        }

        private void OnDisable()
        {
            PartyEvent.OnPartyEvent -= OnPartyEvent;
            needSystem.OnNewNeed -= OnNewNeed;

            AssignedTableSeat.OnFoodArrival -= OnFoodArrival;
            InteractableState.OnPlayerInteract -= OnPlayerInteract;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _blackboard = GetComponent<CharacterBlackboard>();
            InteractableState = GetComponent<GuestInteractable>();

            needSystem = GetComponent<NeedSystem>();
            needSystem.ChangeMood(startingMood);

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            stateMachine = new GuestStateMachine();
            _guestIdleState = new GuestIdleState(this, stateMachine);
            _guestMovingState = new GuestMovingState(this, stateMachine);
            _guestWanderState = new GuestWanderState(this, stateMachine);
            _guestConsumeState = new GuestConsumeState(this, stateMachine);
            _guestGetConsumableState = new GuestGetConsumableState(this, stateMachine);
            _guestMoveToSeatState = new GuestMoveToSeatState(this, stateMachine);

            stateMachine.RegisterState(_guestIdleState);
            stateMachine.RegisterState(_guestMovingState);
            stateMachine.RegisterState(_guestWanderState);
            stateMachine.RegisterState(_guestConsumeState);
            stateMachine.RegisterState(_guestGetConsumableState);
            stateMachine.RegisterState(_guestMoveToSeatState);

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
        }

        public void AssignTableSeat(TableSeat tableSeat)
        {
            tableSeat.AssignSeat();
            AssignedTableSeat = tableSeat;
            SetDestination(tableSeat.transform.position);
        }

        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        public void SetDestinationAndDisplayPath(Vector3 target)
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

        public void OwnerRemoved() {}

        public Transform GetSeatTransform()
        {
            return AssignedTableSeat.GetSeatTransform();
        }

        private void OnNewNeed(INeed need)
        {
            switch (need.GetNeedType())
            {
                case NeedType.Drink:
                    IConsumable consumable = DrinksTable.Instance.TryGetDrink();
                    if (consumable == null)
                    {
                        return;
                    }

                    if (_guestType is GuestType.Famine)
                    {
                        if (Random.Range(0.0f, 1.0f) <= walkToDrinksChance)
                        {
                            CurrentConsumable = consumable;
                            stateMachine.ChangeState(GuestStateID.GetConsumable);
                        }
                    }
                    else if (_guestType is GuestType.Henchmen)
                    {
                        CurrentConsumable = consumable;
                        stateMachine.ChangeState(GuestStateID.GetConsumable);
                    }

                    break;
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
                    needSystem.TryFulfillNeed(NeedType.Music, eventData.needsCost, eventData.moodCost);
                    break;
                case PartyEventType.MusicMachineBreaks:
                    break;
                case PartyEventType.FoodBurning:
                    needSystem.ChangeMood(_guestType == GuestType.Famine
                        ? Mathf.Abs(eventData.moodCost)
                        : eventData.moodCost);
                    break;
                case PartyEventType.PowerOutage:
                    break;
                case PartyEventType.BuntingFall:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnFoodArrival()
        {
            stateMachine.ChangeState(GuestStateID.MoveToSeat);
        }

        private void OnPlayerInteract()
        {
            if (stateMachine.GetCurrentState().GetID() == GuestStateID.Wander)
            {
                stateMachine.ChangeState(GuestStateID.MoveToSeat);
            }
        }
    }
}