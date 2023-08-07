using DG.Tweening;
using UnityEngine;

namespace Minion.States
{
    public class MinionSlipState : MinionState
    {
        private bool _isSlipping;
        public MinionSlipState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Slip;
        }

        public override void Enter()
        {
            _isSlipping = true;

            if (!minion.currentRequest.IsRequestStarted())
            {
                minion.currentRequest.ResetRequest();
                minion.currentRequest.RemoveOwner();
                minion.currentRequest = null;
            }

            minion.NavMeshAgent.ResetPath();
            minion.pathDisplayer.HidePath();

            minion.WaiterTarget?.WaiterCancelled();
            minion.WaiterTarget = null;

            minion.transform.DOShakeRotation(1.5f, new Vector3(0.0f, 0.0f, 40.0f), 5, 1, true,
                    ShakeRandomnessMode.Harmonic)
                .OnComplete(
                    () => { _isSlipping = false; });
            minion.slipAudio.Play(minion.transform.position);
        }

        public override void Tick()
        {
            if (!_isSlipping)
            {
                _stateMachine.ChangeState(MinionStateID.Idle);
            }
        }

        public override void Exit() {}
    }
}