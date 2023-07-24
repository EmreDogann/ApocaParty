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
            minion.image.sprite = minion.actorData.defaultIcon;
            _currentWanderTime = 0.0f;
        }

        public override void Tick()
        {
            if (!minion.IsWandering())
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
                    if (request.IsRequestStarted() || !request.TryStartRequest() || request.GetRequestOwner() != null)
                    {
                        minion.SetWandering(false);
                        _stateMachine.ChangeState(MinionStateID.Idle);

                        return;
                    }

                    switch (request)
                    {
                        case FoodRequest _:
                            // TODO: Play error sound.
                            return;
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
                        default:
                            // TODO: Play error sound.
                            minion.image.sprite = minion.actorData.defaultIcon;
                            break;
                    }

                    minion.SetDestinationAndDisplayPath(request.GetStartingPosition());
                    minion.currentRequest = request;
                    request.AssignOwner(minion);

                    minion.SetWandering(false);
                    _stateMachine.ChangeState(MinionStateID.Moving);
                    break;
                case FridgeInteractable fridgeInteractable:
                    FoodRequest foodRequest = fridgeInteractable.Fridge.TryGetFood();
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