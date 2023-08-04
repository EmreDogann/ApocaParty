using UnityEngine;

namespace Minion.States
{
    public class MinionCleanupState : MinionState
    {
        private float _currentTime;

        public MinionCleanupState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Cleanup;
        }

        public override void Enter()
        {
            minion.NavMeshAgent.ResetPath();
            minion.pathDisplayer.HidePath();

            minion.progressBar.SetProgressBarActive(true);

            _currentTime = 0.0f;
        }

        public override void Tick()
        {
            _currentTime += Time.deltaTime;
            minion.progressBar.SetProgressBarPercentage(_currentTime / minion.cleanupTime);

            if (_currentTime >= minion.cleanupTime)
            {
                minion.TargetConsumable.Cleanup();
                _stateMachine.ChangeState(MinionStateID.Idle);
            }
        }

        public override void Exit()
        {
            minion.progressBar.SetProgressBarActive(false);
            minion.TargetConsumable = null;
        }
    }
}