using UnityEngine;

namespace Guest.States
{
    public class GuestMoveToSeatState : GuestState
    {
        private const float DistanceThreshold = 0.01f;
        public GuestMoveToSeatState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.MoveToSeat;
        }

        public override void Enter()
        {
            guest.SetDestination(guest.GetSeatTransform().position);
        }

        public override void Tick()
        {
            if (guest.HoldingConsumable != null)
            {
                guest.HoldingConsumable.GetTransform().position = guest.GetHoldingPosition().position;
            }

            if (Vector3.SqrMagnitude(guest.transform.position - guest.navMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                if (guest.HoldingConsumable != null)
                {
                    _stateMachine.ChangeState(GuestStateID.Consume);
                }
                else
                {
                    _stateMachine.ChangeState(GuestStateID.Idle);
                }
            }
        }

        public override void Exit() {}
    }
}