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

        public override void Enter() {}

        public override void Tick()
        {
            if (minion.navMeshAgent.hasPath)
            {
                minion.marker.transform.position = minion.navMeshAgent.destination;
            }

            if (Vector3.Distance(minion.transform.position, minion.navMeshAgent.destination) < 0.1f)
            {
                minion.marker.gameObject.SetActive(false);
                _stateMachine.ChangeState(MinionStateID.Idle);
            }
        }

        public override void Exit() {}
    }
}