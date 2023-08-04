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

        public override void Enter() {}

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

        public override void Exit()
        {
            _currentWanderTime = 0.0f;
        }

        public void OnInteraction(InteractableBase interactable)
        {
            if (minion.currentRequest != null || minion.TargetConsumable != null)
            {
                return;
            }

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

                        minion.image.sprite = minion.actorData.eventIcon;
                        _stateMachine.ChangeState(MinionStateID.Moving);
                        return;
                    }

                    if (request.IsRequestStarted() || !request.TryStartRequest() || request.GetRequestOwner() != null)
                    {
                        return;
                    }

                    switch (request)
                    {
                        case DrinkRefillRequest _:
                            if (DrinksTable.Instance.IsDrinksTableFull())
                            {
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

                    _stateMachine.ChangeState(MinionStateID.Moving);
                    break;
                case GuestInteractable guestInteractable:
                    if (guestInteractable.WaiterTarget.HasUnknownRequest() ||
                        !guestInteractable.WaiterTarget.HasConsumable() && minion.HoldingConsumable != null)
                    {
                        minion.SetDestinationAndDisplayPath(guestInteractable.WaiterTarget.GetDestinationTransform()
                            .position);
                        minion.WaiterTarget = guestInteractable.WaiterTarget;
                        minion.WaiterTarget.GiveWaiterID(minion.WaiterID);

                        _stateMachine.ChangeState(MinionStateID.Moving);
                    }
                    else
                    {
                        // TODO: Play error sound.
                        Debug.Log("MinionIdleState - Play Guest Interact Error Sound");
                    }

                    break;
                case FoodPileInteractable foodPileInteractable:
                    FoodRequest foodRequest = foodPileInteractable.FoodPile.TryGetFood();
                    if (foodRequest == null)
                    {
                        // TODO: Play error sound.
                        return;
                    }

                    minion.SetDestinationAndDisplayPath(foodRequest.GetStartingPosition());
                    minion.currentRequest = foodRequest;
                    foodRequest.AssignOwner(minion);

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
                            // TODO: Play error sound.
                            return;
                        }

                        minion.SetDestinationAndDisplayPath(request.GetStartingPosition());
                        minion.currentRequest = request;
                        minion.currentRequest.AssignOwner(minion);

                        minion.image.sprite = minion.actorData.eventIcon;
                        _stateMachine.ChangeState(MinionStateID.Moving);
                    }

                    break;
                case SpillInteractable spillInteractable:
                    minion.SetDestinationAndDisplayPath(spillInteractable.transform.position);
                    minion.TargetConsumable = spillInteractable.Consumable;
                    minion.TargetConsumable.StartCleanup();

                    minion.image.sprite = minion.actorData.eventIcon;
                    _stateMachine.ChangeState(MinionStateID.Moving);
                    break;
                case null:
                    break;
            }
        }
    }
}