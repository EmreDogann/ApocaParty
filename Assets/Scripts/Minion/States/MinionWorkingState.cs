using UnityEngine;

namespace Minion.States
{
    public class MinionWorkingState : MinionState
    {
        public MinionWorkingState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        private bool _isPaused;

        public override MinionStateID GetID()
        {
            return MinionStateID.Working;
        }

        public override void Enter()
        {
            ElectricalBox.OnPowerOutage += OnPowerOutage;
            minion.currentRequest.StartRequest();
        }

        public override void Tick()
        {
            minion.currentRequest.UpdateRequest(Time.deltaTime);
            if (minion.currentRequest.IsRequestCompleted())
            {
                minion.currentRequest = null;
                _stateMachine.ChangeState(MinionStateID.Idle);
            }
        }

        public override void Exit()
        {
            ElectricalBox.OnPowerOutage -= OnPowerOutage;
        }

        private void OnPowerOutage()
        {
            minion.currentRequest = null;
            _stateMachine.ChangeState(MinionStateID.Idle);
        }
    }
}