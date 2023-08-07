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
            guest._isSittingAtSeat = false;
            if (guest.CurrentConsumable == null)
            {
                _stateMachine.ChangeState(GuestStateID.Idle);
            }
            else
            {
                guest.SetDestination(guest.CurrentConsumable.GetTransform().position);
            }
        }

        public override void Tick()
        {
            if (!guest.CurrentConsumable.IsAvailable())
            {
                guest.needSystem.ChangeMood(-1);

                if (guest.CurrentConsumable is Drink && DrinksTable.Instance.IsDrinkAvailable())
                {
                    guest.CurrentConsumable = DrinksTable.Instance.TryGetDrink();
                    if (guest.CurrentConsumable != null)
                    {
                        guest.SetDestination(guest.CurrentConsumable.GetTransform().position);
                    }
                }
                else
                {
                    guest.CurrentConsumable = null;
                    _stateMachine.ChangeState(GuestStateID.MoveToSeat);
                }

                return;
            }

            if (Vector3.SqrMagnitude(guest.transform.position - guest.navMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                guest.CurrentConsumable.SetSorting(guest.spriteRenderer.sortingLayerID,
                    guest.spriteRenderer.sortingOrder + 1);
                guest.CurrentConsumable.Claim();
                _stateMachine.ChangeState(GuestStateID.MoveToSeat);
            }
        }

        public override void Exit()
        {
            if (guest.GuestType == GuestType.Famine && guest.CurrentConsumable is Drink)
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