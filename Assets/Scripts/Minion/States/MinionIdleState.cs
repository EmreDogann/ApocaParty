using UnityEngine;

namespace Minion.States
{
    public class MinionIdleState : MinionState
    {
        public MinionIdleState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Idle;
        }

        public override void Enter() {}

        public override void Tick()
        {
            // if (Mouse.current.rightButton.wasPressedThisFrame)
            // {
            //     minion.navMeshAgent.SetDestination(minion._mainCamera.ScreenToWorldPoint(Mouse.current.position.value));
            //     minion.marker.transform.position = minion.navMeshAgent.destination;
            //     _stateMachine.ChangeState(MinionStateID.Working);
            //
            //     if (minion.showPath)
            //     {
            //         minion.marker.gameObject.SetActive(true);
            //     }
            // }

            if (minion.InteractableState.IsHovering)
            {
                minion.transform.localScale = Vector3.one * 1.5f;
            }
            else
            {
                minion.transform.localScale = Vector3.one;
            }

            if (minion.InteractableState.IsInteracting)
            {
                _stateMachine.ChangeState(MinionStateID.Assignment);
            }
        }

        public override void Exit() {}
    }
}