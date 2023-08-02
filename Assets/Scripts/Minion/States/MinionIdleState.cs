using Consumable;
using GuestRequests;
using GuestRequests.Requests;
using Interactions;
using Interactions.Interactables;
using UnityEngine;

namespace Minion.States
{
    public class MinionIdleState : MinionState
    {
        private const float TimeToWander = 3.0f;
        private float _currentWanderTime;
        public MinionIdleState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Idle;
        }

        public override void Enter()
        {
            _currentWanderTime = 0.0f;
        }

        public override void Tick()
        {
            if (minion.HoldingConsumable != null)
            {
                minion.HoldingConsumable.GetTransform().position = minion.GetHoldingTransform().position;
            }

            if (minion.enableWandering && !minion.IsWandering())
            {
                _currentWanderTime += Time.deltaTime;
            }

            if (_currentWanderTime >= TimeToWander)
            {
                minion.SetWandering(true);
                _currentWanderTime = 0.0f;
            }
        }

        public override void Exit() {}

        public void OnInteraction(InteractableBase interactable)
        {
            switch (interactable)
            {
                case IInteractableRequest requestInteractable:
                    Request request = requestInteractable.GetRequest();

                    if (request is FoodRequest && request.GetRequestOwner() == null)
                    {
                        if (minion.HoldingConsumable == null && request.IsRequestCompleted())
                        {
                            minion.TargetConsumable = request as IConsumable;
                            minion.TargetConsumable.Claim();
                        }

                        minion.SetDestinationAndDisplayPath(request.GetStartingPosition());

                        minion.SetWandering(false);
                        minion.image.sprite = minion.actorData.eventIcon;
                        _stateMachine.ChangeState(MinionStateID.Moving);
                        return;
                    }

                    if (request.IsRequestStarted() || !request.TryStartRequest() || request.GetRequestOwner() != null)
                    {
                        minion.SetWandering(false);
                        _stateMachine.ChangeState(MinionStateID.Idle);

                        return;
                    }

                    switch (request)
                    {
                        case DrinkRefillRequest _:
                            if (DrinksTable.Instance.IsDrinksTableFull())
                            {
                                _stateMachine.ChangeState(MinionStateID.Idle);
                                return;
                            }

                            minion.image.sprite = minion.actorData.kitchenIcon;
                            break;
                        case MusicRequest _:
                            minion.image.sprite = minion.actorData.musicIcon;
                            break;
                        case BuntingRequest _:
                            minion.image.sprite = minion.actorData.eventIcon;
                            break;
                    }

                    minion.SetDestinationAndDisplayPath(request.GetStartingPosition());
                    minion.currentRequest = request;
                    request.AssignOwner(minion);

                    minion.SetWandering(false);
                    _stateMachine.ChangeState(MinionStateID.Moving);
                    break;
                case GuestInteractable guestInteractable:
                    minion.SetDestinationAndDisplayPath(guestInteractable.WaiterTarget.GetDestinationTransform()
                        .position);
                    guestInteractable.WaiterTarget.GiveWaiterID(minion.WaiterID);

                    minion.SetWandering(false);
                    _stateMachine.ChangeState(MinionStateID.Moving);
                    break;
                case FoodPileInteractable foodPileInteractable:
                    FoodRequest foodRequest = foodPileInteractable.FoodPile.TryGetFood();
                    if (foodRequest == null)
                    {
                        // TODO: Play error sound.
                        _stateMachine.ChangeState(MinionStateID.Idle);
                        return;
                    }

                    minion.SetDestinationAndDisplayPath(foodRequest.GetStartingPosition());
                    minion.currentRequest = foodRequest;
                    foodRequest.AssignOwner(minion);

                    minion.SetWandering(false);
                    minion.image.sprite = minion.actorData.kitchenIcon;
                    _stateMachine.ChangeState(MinionStateID.Moving);
                    break;
                case DrinksTableInteractable drinksTableInteractable:
                    if (drinksTableInteractable.IsDrinkAvailable())
                    {
                        if (minion.HoldingConsumable == null)
                        {
                            minion.TargetConsumable = drinksTableInteractable.TryGetDrink();
                            if (minion.TargetConsumable != null)
                            {
                                minion.SetDestinationAndDisplayPath(minion.TargetConsumable.GetTransform().position);

                                minion.SetWandering(false);
                                minion.image.sprite = minion.actorData.eventIcon;
                                _stateMachine.ChangeState(MinionStateID.Moving);
                            }
                        }
                    }
                    else
                    {
                        request = drinksTableInteractable.TryRefill();
                        if (request == null)
                        {
                            return;
                        }

                        minion.SetDestinationAndDisplayPath(request.GetStartingPosition());
                        minion.currentRequest = request;
                        minion.currentRequest.AssignOwner(minion);

                        minion.SetWandering(false);
                        minion.image.sprite = minion.actorData.eventIcon;
                        _stateMachine.ChangeState(MinionStateID.Moving);
                    }

                    break;
                case null:
                    _stateMachine.ChangeState(MinionStateID.Idle);
                    break;
                default:
                    _stateMachine.ChangeState(MinionStateID.Idle);
                    break;
            }
        }
    }
}