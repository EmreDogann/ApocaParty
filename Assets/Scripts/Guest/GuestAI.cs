using Actors;
using Audio;
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
        public SpriteRenderer spriteRenderer;
        [SerializeField] private Transform holderTransform;
        public ActorSO actorData;

        [Separator("Moods")]
        [SerializeField] private MoodType startingMood;

        [Separator("AI Behaviour")]
        [SerializeField] private bool activateAIOnAwake;
        [Range(0.0f, 1.0f)] public float chanceToSpillDrink;
        public float spillDrinkCheckFrequency;
        [Range(0.0f, 1.0f)] public float walkToDrinksChance;

        [Separator("UI")]
        public ProgressBar consumeProgressBar;

        [Separator("Audio")]
        [SerializeField] private AudioSO errorSound;

        public NeedSystem needSystem { get; private set; }
        public Camera _mainCamera { get; private set; }
        [field: SerializeReference] public TableSeat AssignedTableSeat { get; private set; }

        private GuestIdleState _guestIdleState;
        private GuestConsumeState _guestConsumeState;
        private GuestGetConsumableState _guestGetConsumableState;
        private GuestMoveToSeatState _guestMoveToSeatState;

        public IConsumable CurrentConsumable;

        private int _waiterID;
        private bool _isAssignedWaiter;
        internal bool _isSittingAtSeat;
        internal bool IsInSpillZone;
        public bool TutorialMode { get; private set; }

        private bool _isAIActive;

        private int _spillZoneLayer;

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
            _guestConsumeState = new GuestConsumeState(this, stateMachine);
            _guestGetConsumableState = new GuestGetConsumableState(this, stateMachine);
            _guestMoveToSeatState = new GuestMoveToSeatState(this, stateMachine);

            stateMachine.RegisterState(_guestIdleState);
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

            _spillZoneLayer = LayerMask.NameToLayer("SpillZone");
        }

        private void OnEnable()
        {
            PartyEvent.OnPartyEvent += OnPartyEvent;

            needSystem.OnNewNeed += OnNewNeed;
            needSystem.OnNeedFulfilled += OnNeedFulfilled;
            needSystem.OnNeedExpired += OnNeedExpired;
        }

        private void OnDisable()
        {
            PartyEvent.OnPartyEvent -= OnPartyEvent;

            needSystem.OnNewNeed -= OnNewNeed;
            needSystem.OnNeedFulfilled -= OnNeedFulfilled;
            needSystem.OnNeedExpired -= OnNeedExpired;
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f || !_isAIActive)
            {
                return;
            }

            stateMachine.UpdateState();
        }

        private void OnNeedFulfilled(NeedType needType)
        {
            if (_guestType != GuestType.Henchmen)
            {
                VibeMeter.ChangeVibe.Invoke(15, !TutorialMode);
            }
            else
            {
                VibeMeter.ChangeVibe.Invoke(5, !TutorialMode);
            }
        }

        private void OnNeedExpired(NeedType needType)
        {
            if (_guestType != GuestType.Henchmen)
            {
                VibeMeter.ChangeVibe.Invoke(-10, !TutorialMode);
            }
            else
            {
                VibeMeter.ChangeVibe.Invoke(-2, !TutorialMode);
            }
        }

        public void AssignTableSeat(TableSeat tableSeat, bool goToSeat)
        {
            tableSeat.AssignSeat();
            AssignedTableSeat = tableSeat;
            if (goToSeat)
            {
                stateMachine.ChangeState(GuestStateID.MoveToSeat);
            }
        }

        public void WarpToSeat()
        {
            navMeshAgent.Warp(AssignedTableSeat.GetSeatTransform().position);
            _isSittingAtSeat = true;
        }

        public bool IsSittingAtSeat()
        {
            return _isSittingAtSeat;
        }

        public void ActivateAI()
        {
            _isAIActive = true;
        }

        public void SetActiveTutorialMode(bool isActive)
        {
            TutorialMode = isActive;
            needSystem.SetTutorialMode(isActive);
            if (!_isAIActive)
            {
                ActivateAI();
            }
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

        public CharacterType GetOwnerType()
        {
            return CharacterType.Guest;
        }

        public Transform GetSeatTransform()
        {
            return AssignedTableSeat.GetSeatTransform();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.layer == _spillZoneLayer)
            {
                IsInSpillZone = true;
            }
            else
            {
                IsInSpillZone = false;
            }
        }

        private void OnNewNeed(INeed need)
        {
            if (TutorialMode)
            {
                return;
            }

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

                            needSystem.TryResolveNeed(need.GetNeedType());
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

                    if (_guestType == GuestType.Henchmen)
                    {
                        if (stateMachine.GetCurrentState().GetID() == GuestStateID.GetConsumable &&
                            CurrentConsumable is Drink)
                        {
                            CurrentConsumable = null;
                            stateMachine.ChangeState(GuestStateID.MoveToSeat);
                        }
                    }

                    if (_guestType == GuestType.Famine)
                    {
                        VibeMeter.ChangeVibe.Invoke(12, false);
                    }
                    else
                    {
                        VibeMeter.ChangeVibe.Invoke(-3, !TutorialMode);
                    }

                    return;
                case PartyEventType.MusicPlaying:
                    needSystem.TryFulfillNeed(NeedType.Music, eventData.needsCost, eventData.moodCost);
                    return;
                case PartyEventType.MusicMachineBreaks:
                    needSystem.ChangeMood(eventData.moodCost);
                    break;
                case PartyEventType.FoodBurning:
                    // needSystem.ChangeMood(eventData.moodCost);
                    break;
                case PartyEventType.PowerOutage:
                    needSystem.ChangeMood(eventData.moodCost);
                    break;
                case PartyEventType.BuntingFall:
                    needSystem.ChangeMood(eventData.moodCost);
                    break;
            }

            if (_guestType != GuestType.Henchmen)
            {
                VibeMeter.ChangeVibe.Invoke(-5, !TutorialMode);
            }
            else
            {
                VibeMeter.ChangeVibe.Invoke(-3, !TutorialMode);
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

        public bool HasNeed(NeedType needType)
        {
            return needSystem.HasNeed(needType);
        }

        public bool HasConsumable()
        {
            return CurrentConsumable != null || AssignedTableSeat != null && AssignedTableSeat.HasFood();
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
                if (consumable != null && stateMachine.GetCurrentState().GetID() == GuestStateID.Idle)
                {
                    switch (consumable)
                    {
                        case FoodRequest _:
                            consumable.GetTransform().position = AssignedTableSeat.GetPlateTransform().position;
                            AssignedTableSeat.SetFood(consumable);
                            break;
                        case Drink _:
                            CurrentConsumable = consumable;
                            CurrentConsumable.SetSorting(spriteRenderer.sortingLayerID,
                                spriteRenderer.sortingOrder + 1);
                            break;
                    }

                    stateMachine.ChangeState(GuestStateID.Consume);
                    _waiterID = 0;
                    _isAssignedWaiter = false;
                    return;
                }

                errorSound.Play(transform.position);
            }

            _waiterID = 0;
            _isAssignedWaiter = false;
            InteractableState.SetInteractableActive(true);
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