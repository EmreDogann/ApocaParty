using Electricity;
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

            minion.currentRequest.OnRequestCompleted += OnRequestCompleted;
            minion.currentRequest.ActivateRequest();

            minion.progressBar.SetProgressBarActive(true);
        }

        public override void Tick()
        {
            if (_isPowerOut)
            {
                return;
            }

            minion.currentRequest.UpdateRequest(Time.deltaTime);
            if (minion.currentRequest)
            {
                minion.progressBar.SetProgressBarPercentage(minion.currentRequest.GetProgress());
            }
        }

        public override void Exit()
        {
            ElectricalBox.OnPowerOutage -= OnPowerOutage;
            ElectricalBox.OnPowerFixed -= OnPowerFixed;

            minion.progressBar.SetProgressBarActive(false);

            minion.NavMeshAgent.SetDestination(minion.RandomNavmeshLocation(minion.transform.position,
                minion.SearchRadius * 0.2f,
                minion.NavMeshAgent.areaMask));
        }

        private void OnRequestCompleted()
        {
            minion.currentRequest.OnRequestCompleted -= OnRequestCompleted;
            minion.currentRequest = null;
            _stateMachine.ChangeState(MinionStateID.Idle);
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