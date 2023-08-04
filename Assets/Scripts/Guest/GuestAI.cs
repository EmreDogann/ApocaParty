using Actors;
using Consumable;
using Dialogue;
using DiningTable;
using Guest.States;
using GuestRequests;
using GuestRequests.Requests;
using Interactions.Interactables;
using MyBox;
using Needs;
using PartyEvents;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using Utils;

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

    [RequireComponent(typeof(NavMeshAgent), typeof(GuestInteractable), typeof(NeedSystem))]
    public class GuestAI : MonoBehaviour, IGuestRequestOwner, IWaiterTarget
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
        [NavMeshSelector] [SerializeField] private int ignoreAreaCosts;

        [Separator("Guest Data")]
        public SpriteRenderer image;
        [SerializeField] private Transform holderTransform;
        public ActorSO actorData;

        [Separator("Moods")]
        [SerializeField] private MoodType startingMood;

        [Separator("AI Behaviour")]
        [SerializeField] private bool activateAIOnAwake;

        [Range(0.0f, 1.0f)] public float chanceToSpillDrink;
        public float spillDrinkCheckFrequency;
        [Range(0.0f, 1.0f)] public float walkToDrinksChance;

        [Separator("Debugging")]
        [SerializeField] private TextMeshProUGUI AIState;

        public NeedSystem needSystem { get; private set; }
        public Camera _mainCamera { get; private set; }

        private GuestIdleState _guestIdleState;
        private GuestMovingState _guestMovingState;
        private GuestConsumeState _guestConsumeState;
        private GuestGetConsumableState _guestGetConsumableState;
        private GuestMoveToSeatState _guestMoveToSeatState;

        public IConsumable CurrentConsumable;
        [field: SerializeReference] public TableSeat AssignedTableSeat { get; private set; }

        private int _waiterID;
        private bool _isAssignedWaiter;

        private bool _isAIActive;

        private void Awake()
        {
            _mainCamera = Camera.main;
            InteractableState = GetComponent<GuestInteractable>();

            needSystem = GetComponent<NeedSystem>();
            needSystem.ChangeMood(startingMood);

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            stateMachine = new GuestStateMachine();
            _guestIdleState = new GuestIdleState(this, stateMachine);
            _guestMovingState = new GuestMovingState(this, stateMachine);
            _guestConsumeState = new GuestConsumeState(this, stateMachine);
            _guestGetConsumableState = new GuestGetConsumableState(this, stateMachine);
            _guestMoveToSeatState = new GuestMoveToSeatState(this, stateMachine);

            stateMachine.RegisterState(_guestIdleState);
            stateMachine.RegisterState(_guestMovingState);
            stateMachine.RegisterState(_guestConsumeState);
            stateMachine.RegisterState(_guestGetConsumableState);
            stateMachine.RegisterState(_guestMoveToSeatState);

            // Initial state
            stateMachine.ChangeState(GuestStateID.Idle);

            navMeshAgent.SetAreaCost(ignoreAreaCosts, 1.0f);

            if (activateAIOnAwake)
            {
                ActivateAI();
            }
        }

        private void OnEnable()
        {
            VibeMeter.VibeCheck += VibeCheck;
            PartyEvent.OnPartyEvent += OnPartyEvent;
            needSystem.OnNewNeed += OnNewNeed;
        }

        private void OnDisable()
        {
            VibeMeter.VibeCheck -= VibeCheck;
            PartyEvent.OnPartyEvent -= OnPartyEvent;
            needSystem.OnNewNeed -= OnNewNeed;
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f || !_isAIActive)
            {
                return;
            }

            stateMachine.UpdateState();
            AIState.text = stateMachine.GetCurrentState().GetID().ToString();
        }

        private void VibeCheck()
        {
            if (_guestType != GuestType.Henchmen)
            {
                VibeMeter.ChangeVibe.Invoke(needSystem.IsSatisfied() ? 5 : -5);
            }
        }

        public void AssignTableSeat(TableSeat tableSeat, bool goToSeat)
        {
            tableSeat.AssignSeat();
            AssignedTableSeat = tableSeat;
            if (goToSeat)
            {
                SetDestination(tableSeat.transform.position);
            }
        }

        public void ActivateAI()
        {
            _isAIActive = true;
        }

        public void SetDestination(Vector3 target)
        {
            navMeshAgent.SetDestination(target);
        }

        public void SetDestinationAndDisplayPath(Vector3 target)
        {
            SetDestination(target);
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Transform GetHoldingTransform()
        {
            return holderTransform;
        }

        public void OwnerRemoved() {}

        public OwnerType GetOwnerType()
        {
            return OwnerType.Guest;
        }

        public Transform GetSeatTransform()
        {
            return AssignedTableSeat.GetSeatTransform();
        }

        private void OnNewNeed(INeed need)
        {
            switch (need.GetNeedType())
            {
                case NeedType.Drink:
                    if (!DrinksTable.Instance.IsDrinkAvailable())
                    {
                        return;
                    }

                    if (_guestType is GuestType.Famine or GuestType.Henchmen)
                    {
                        if (Random.Range(0.0f, 1.0f) <= walkToDrinksChance)
                        {
                            CurrentConsumable = DrinksTable.Instance.TryGetDrink();
                            stateMachine.ChangeState(GuestStateID.GetConsumable);
                            InteractableState.SetInteractableActive(false);
                        }
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
                    needSystem.ChangeMood(eventData.moodCost);
                    break;
                case PartyEventType.FoodBurning:
                    needSystem.ChangeMood(_guestType == GuestType.Famine
                        ? Mathf.Abs(eventData.moodCost)
                        : eventData.moodCost);
                    break;
                case PartyEventType.PowerOutage:
                    needSystem.ChangeMood(eventData.moodCost);
                    break;
                case PartyEventType.BuntingFall:
                    needSystem.ChangeMood(eventData.moodCost);
                    break;
            }
        }

        private void OnWaiterInteractDialogueFinished()
        {
            needSystem.ResolveNeeds();
        }

        public bool HasUnknownRequest()
        {
            return _guestType != GuestType.Henchmen && needSystem.HasUnresolvedNeed();
        }

        public bool HasConsumable()
        {
            return CurrentConsumable != null || AssignedTableSeat.HasFood();
        }

        public void WaiterInteracted(IWaiter waiter)
        {
            var messages = needSystem.GetUnknownNeedConversations();
            if (_guestType != GuestType.Henchmen && messages.Count > 0)
            {
                foreach (Message message in messages)
                {
                    message.actor = actorData;
                }

                DialogueManager.Instance.OpenRandomDialogue(messages.ToArray(),
                    OnWaiterInteractDialogueFinished);
            }
            else
            {
                IConsumable consumable = waiter.GetConsumable();
                if (consumable != null)
                {
                    switch (consumable)
                    {
                        case FoodRequest _:
                            consumable.GetTransform().position = AssignedTableSeat.GetPlateTransform().position;
                            AssignedTableSeat.SetFood(consumable);
                            break;
                        case Drink _:
                            if (stateMachine.GetCurrentState().GetID() == GuestStateID.GetConsumable)
                            {
                                return;
                            }

                            CurrentConsumable = consumable;
                            break;
                    }

                    if (stateMachine.GetCurrentState().GetID() != GuestStateID.MoveToSeat)
                    {
                        stateMachine.ChangeState(GuestStateID.MoveToSeat);
                    }
                }
            }

            _waiterID = 0;
            _isAssignedWaiter = false;
        }

        public void WaiterCancelled()
        {
            _waiterID = 0;
            _isAssignedWaiter = false;
            InteractableState.SetInteractableActive(true);
        }

        public bool IsAssignedWaiter()
        {
            return _isAssignedWaiter;
        }

        public void GiveWaiterID(int waiterID)
        {
            _waiterID = waiterID;
            _isAssignedWaiter = true;
            InteractableState.SetInteractableActive(false);
        }

        public int GetWaiterID()
        {
            return _waiterID;
        }

        public Transform GetDestinationTransform()
        {
            return AssignedTableSeat.GetDestinationTransform();
        }
    }
}