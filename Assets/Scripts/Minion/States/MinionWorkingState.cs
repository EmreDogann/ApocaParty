using UnityEngine;

namespace Minion.States
{
    public class MinionWorkingState : MinionState
    {
        public MinionWorkingState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Working;
        }

        public override void Enter()
        {
            minion.currentRequest.StartRequest();
        }

        public override void Tick()
        {
            minion.currentRequest.UpdateRequest(Time.deltaTime);
            if (minion.currentRequest.IsRequestCompleted())
            {
                _stateMachine.ChangeState(MinionStateID.Idle);
            }
        }

        public override void Exit() {}
    }
}