using System;
using Interactions;
using JetBrains.Annotations;
using UnityEngine;

namespace Minion.States
{
    public class MinionAssignmentState : MinionState
    {
        private Action<InteractableBase> _onInteractionCallback;

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
            minion.transform.localScale = Vector3.one * 1.5f;

            InteractionSystem.EnableAssignmentMode(_onInteractionCallback);
        }

        public override void Tick()
        {
            // if (!minion.InteractableState.IsInteracting)
            // {
            //     _stateMachine.ChangeState(MinionStateID.Idle);
            // }
        }

        public override void Exit()
        {
            minion.transform.localScale = Vector3.one;
            InteractionSystem.DisableAssignmentMode();
        }

        private void OnInteraction([CanBeNull] InteractableBase interactable)
        {
            if (interactable != null)
            {
                if (interactable is IInteractableRequest)
                {
                    minion.navMeshAgent.SetDestination(interactable.transform.position);
                    _stateMachine.ChangeState(MinionStateID.Working);
                }
            }
            else
            {
                _stateMachine.ChangeState(MinionStateID.Idle);
            }
        }
    }
}