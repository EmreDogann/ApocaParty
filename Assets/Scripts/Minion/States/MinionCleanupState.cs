using Interactions.Interactables;
using UnityEngine;

namespace Minion.States
{
    public class MinionCleanupState : MinionState
    {
        private float _currentTime;

        private readonly Collider2D[] _cleanupRaycastHits;
        private readonly ContactFilter2D _cleanupContactFilter;

        public MinionCleanupState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine)
        {
            _cleanupRaycastHits = new Collider2D[minion.maxCleanupAmount];
            _cleanupContactFilter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true,
                layerMask = 1 << minion.SpillLayer
            };
        }

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
                int hitCount = Physics2D.OverlapCircle(minion.transform.position, 0.7f, _cleanupContactFilter,
                    _cleanupRaycastHits);

                if (hitCount > 0)
                {
                    for (int i = 0; i < hitCount; i++)
                    {
                        SpillInteractable spill = _cleanupRaycastHits[i].GetComponent<SpillInteractable>();
                        if (spill != null && spill.Consumable.IsSpilled())
                        {
                            spill.Consumable.Cleanup();
                        }
                    }
                }

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