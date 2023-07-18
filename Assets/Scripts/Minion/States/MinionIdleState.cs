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

        public override void Enter()
        {
            minion.image.sprite = minion.actorData.defaultIcon;
        }

        public override void Tick()
        {
            if (minion.InteractableState.IsHovering)
            {
                minion.transform.localScale = Vector3.one * 1.2f;
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