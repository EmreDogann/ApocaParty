using UnityEngine.InputSystem;

namespace Minion.States
{
    public class MinionIdleState : MinionState
    {
        public MinionIdleState(Minion minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Idle;
        }

        public override void Enter() {}

        public override void Tick()
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                minion.navMeshAgent.SetDestination(minion._mainCamera.ScreenToWorldPoint(Mouse.current.position.value));
                minion.marker.transform.position = minion.navMeshAgent.destination;
                _stateMachine.ChangeState(MinionStateID.Travelling);

                if (minion.showPath)
                {
                    minion.marker.gameObject.SetActive(true);
                }
            }
        }

        public override void Exit() {}
    }
}