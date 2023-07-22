using UnityEngine;

namespace Minion.States
{
    public class MinionWorkingState : MinionState
    {
        public MinionWorkingState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        private bool _isPowerOut;

        public override MinionStateID GetID()
        {
            return MinionStateID.Working;
        }

        public override void Enter()
        {
            ElectricalBox.OnPowerOutage += OnPowerOutage;
            ElectricalBox.OnPowerFixed += OnPowerFixed;
            minion.currentRequest.ActivateRequest();
        }

        public override void Tick()
        {
            if (_isPowerOut)
            {
                return;
            }

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
            ElectricalBox.OnPowerFixed -= OnPowerFixed;
        }

        private void OnPowerOutage()
        {
            _isPowerOut = true;
        }

        private void OnPowerFixed()
        {
            _isPowerOut = false;
        }
    }
}