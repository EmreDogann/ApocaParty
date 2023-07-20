using Consumable;
using PartyEvents;
using Unity.VisualScripting;
using UnityEngine;

namespace Guest.States
{
    public class GuestGetConsumableState : GuestState
    {
        private const float DistanceThreshold = 0.1f;

        public GuestGetConsumableState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.GetConsumable;
        }

        public override void Enter()
        {
            if (guest.HoldingConsumable == null)
            {
                _stateMachine.ChangeState(GuestStateID.Idle);
            }
            else
            {
                guest.SetDestination(guest.HoldingConsumable.GetTransform().position);
            }
        }

        public override void Tick()
        {
            if (!guest.HoldingConsumable.IsAvailable())
            {
                guest.needSystem.ChangeMood(-1);
                guest.HoldingConsumable = null;
                _stateMachine.ChangeState(GuestStateID.MoveToSeat);
                return;
            }

            if (Vector3.SqrMagnitude(guest.transform.position - guest.navMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                guest.HoldingConsumable.Claim();
                _stateMachine.ChangeState(GuestStateID.MoveToSeat);
            }
        }

        public override void Exit()
        {
            if (guest.GuestType == GuestType.Famine && guest.HoldingConsumable is Drink)
            {
                FamineEvent famineEvent = guest.GetComponent<FamineEvent>();
                if (!famineEvent)
                {
                    guest.AddComponent<FamineEvent>();
                }

                famineEvent.TriggerEvent();
            }
        }
    }
}