using Consumable;
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

        public override void Enter()
        {
            minion.SetWandering(false);
        }

        public override void Tick()
        {
            if (minion.TargetConsumable != null && !minion.TargetConsumable.IsAvailable())
            {
                if (minion.TargetConsumable is Drink && DrinksTable.Instance.IsDrinkAvailable())
                {
                    minion.TargetConsumable = DrinksTable.Instance.TryGetDrink();
                    if (minion.TargetConsumable != null)
                    {
                        minion.SetDestination(minion.TargetConsumable.GetTransform().position);
                    }
                }
                else
                {
                    minion.TargetConsumable = null;
                    minion.NavMeshAgent.ResetPath();
                    minion.pathDisplayer.HidePath();
                    _stateMachine.ChangeState(MinionStateID.Idle);
                    _stateMachine.ChangeState(MinionStateID.Idle);
                    return;
                }
            }

            if (Vector3.SqrMagnitude(minion.transform.position - minion.NavMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                if (minion.TargetConsumable != null)
                {
                    if (minion.TargetConsumable.IsSpilled())
                    {
                        _stateMachine.ChangeState(MinionStateID.Cleanup);
                        return;
                    }

                    minion.HoldingConsumable = minion.TargetConsumable;
                    minion.HoldingConsumable.Claim();
                    minion.HoldingConsumable.SetSorting(minion.image.sortingLayerID, minion.image.sortingOrder + 1);
                    minion.TargetConsumable = null;

                    minion.NavMeshAgent.SetDestination(minion.RandomNavmeshLocation(minion.transform.position,
                        minion.SearchRadius * 0.6f, minion.NavMeshAgent.areaMask));
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