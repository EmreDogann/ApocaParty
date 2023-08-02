using UnityEngine;

namespace Minion.States
{
    public class MinionMovingState : MinionState
    {
        private const float DistanceThreshold = 0.01f;
        public MinionMovingState(MinionAI minion, MinionStateMachine stateMachine) : base(minion, stateMachine) {}

        public override MinionStateID GetID()
        {
            return MinionStateID.Moving;
        }

        public override void Enter() {}

        public override void Tick()
        {
            if (Vector3.SqrMagnitude(minion.transform.position - minion.NavMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                if (minion.TargetConsumable != null && !minion.TargetConsumable.IsSpilled())
                {
                    minion.HoldingConsumable = minion.TargetConsumable;
                    minion.HoldingConsumable.Claim();
                    minion.HoldingConsumable.SetSorting(minion.image.sortingLayerID, minion.image.sortingOrder + 1);
                    minion.TargetConsumable = null;
                }

                _stateMachine.ChangeState(minion.currentRequest ? MinionStateID.Working : MinionStateID.Idle);
            }

            if (minion.HoldingConsumable != null)
            {
                minion.HoldingConsumable.GetTransform().position = minion.GetHoldingTransform().position;
            }
        }

        public override void Exit()
        {
            minion.pathDisplayer.HidePath();
        }
    }
}