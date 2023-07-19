using UnityEngine;
using UnityEngine.AI;

namespace Minion.States
{
    public class MinionMovingState : MinionState
    {
        private const float DistanceThreshold = 0.1f;
        public MinionMovingState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Moving;
        }

        public override void Enter()
        {
            if (minion.showPath)
            {
                NavMeshPath path = minion.navMeshAgent.path;
                minion.pathRenderer.positionCount = path.corners.Length;
                minion.pathRenderer.SetPositions(path.corners);
                minion.marker.gameObject.SetActive(true);
            }
        }

        public override void Tick()
        {
            if (minion.navMeshAgent.hasPath)
            {
                minion.marker.transform.position = minion.navMeshAgent.destination;
            }

            if (Vector3.SqrMagnitude(minion.transform.position - minion.navMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                minion.marker.gameObject.SetActive(false);
                _stateMachine.ChangeState(minion.currentRequest ? MinionStateID.Working : MinionStateID.Idle);
            }
        }

        public override void Exit()
        {
            minion.pathRenderer.positionCount = 0;
            minion.marker.gameObject.SetActive(false);
        }
    }
}