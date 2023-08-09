using UnityEngine;

namespace Guest.States
{
    public class GuestMoveToSeatState : GuestState
    {
        private const float DistanceThreshold = 0.1f;
        private float _currentTime;
        public GuestMoveToSeatState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.MoveToSeat;
        }

        public override void Enter()
        {
            guest.SetDestination(guest.GetSeatTransform().position);
            _currentTime = 0.0f;
        }

        public override void Tick()
        {
            _currentTime += Time.deltaTime;
            if (guest.CurrentConsumable != null)
            {
                guest.CurrentConsumable.GetTransform().position = guest.GetHoldingTransform().position;
                if (_currentTime > guest.spillDrinkCheckFrequency)
                {
                    _currentTime = 0.0f;
                    if (guest.IsInSpillZone && Random.Range(0.0f, 1.0f) <= guest.chanceToSpillDrink)
                    {
                        guest.CurrentConsumable.Spill();
                        guest.CurrentConsumable = null;
                    }
                }
            }

            if (Vector3.SqrMagnitude(guest.transform.position - guest.navMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                if (guest.CurrentConsumable != null || guest.AssignedTableSeat.IsFoodAvailable())
                {
                    _stateMachine.ChangeState(GuestStateID.Consume);
                }
                else
                {
                    _stateMachine.ChangeState(GuestStateID.Idle);
                }
            }
        }

        public override void Exit()
        {
            guest._isSittingAtSeat = true;
        }
    }
}