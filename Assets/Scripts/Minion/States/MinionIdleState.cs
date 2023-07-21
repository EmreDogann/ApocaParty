using UnityEngine;

namespace Minion.States
{
    public class MinionIdleState : MinionState
    {
        private const float TimeToWander = 3.0f;
        private float _currentWanderTime;
        public MinionIdleState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Idle;
        }

        public override void Enter()
        {
            minion.image.sprite = minion.actorData.defaultIcon;
            _currentWanderTime = 0.0f;
        }

        public override void Tick()
        {
            if (!minion.IsWandering())
            {
                _currentWanderTime += Time.deltaTime;
            }

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

            if (_currentWanderTime >= TimeToWander)
            {
                minion.SetWandering(true);
                _currentWanderTime = 0.0f;
            }
        }

        public override void Exit() {}
    }
}