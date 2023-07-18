using System;
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
                    minion.navMeshAgent.SetDestination(interactable.transform.position);
                    minion.currentRequest = requestInteractable.GetRequest();
                    minion.currentRequest.AssignOwner(minion);

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