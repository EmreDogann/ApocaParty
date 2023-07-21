using System;
using GuestRequests;
using GuestRequests.Requests;
using Interactions;
using UnityEngine;

namespace Minion.States
{
    public class MinionAssignmentState : MinionState
    {
        private Action<InteractableBase> _onInteractionCallback;
        public static event Action<Action<InteractableBase>> OnMinionInteract;

        public MinionAssignmentState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine)
        {
            _onInteractionCallback += OnInteraction;
        }

        ~MinionAssignmentState()
        {
            _onInteractionCallback -= OnInteraction;
        }

        public override MinionStateID GetID()
        {
            return MinionStateID.Assignment;
        }

        public override void Enter()
        {
            minion.transform.localScale = Vector3.one * 1.2f;

            OnMinionInteract?.Invoke(_onInteractionCallback);
        }

        public override void Tick() {}

        public override void Exit()
        {
            minion.transform.localScale = Vector3.one;
        }

        private void OnInteraction(InteractableBase interactable)
        {
            switch (interactable)
            {
                case IInteractableRequest requestInteractable:
                    Request request = requestInteractable.GetRequest();
                    if (request.IsRequestCompleted())
                    {
                        break;
                    }

                    switch (request)
                    {
                        case FoodRequest _:
                            if (!request.TryAcquireRequestDependencies())
                            {
                                minion.SetWandering(false);
                                _stateMachine.ChangeState(MinionStateID.Idle);

                                return;
                            }

                            minion.image.sprite = minion.actorData.kitchenIcon;
                            break;
                        case DrinkRefillRequest _:
                            minion.image.sprite = minion.actorData.kitchenIcon;
                            break;
                        case MusicRequest _:
                            minion.image.sprite = minion.actorData.musicIcon;
                            break;
                        case EventRequest _:
                            minion.image.sprite = minion.actorData.eventIcon;
                            break;
                        default:
                            // TODO: Play error sound.
                            minion.image.sprite = minion.actorData.defaultIcon;
                            break;
                    }

                    minion.navMeshAgent.SetDestination(interactable.transform.position);
                    minion.currentRequest = request;
                    request.AssignOwner(minion);

                    minion.SetWandering(false);

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