using GuestRequests.Requests;
using UnityEngine;

namespace Guest.States
{
    public class GuestIdleState : GuestState
    {
        private float _currentWanderTime;
        private bool isFoodComing;
        public GuestIdleState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Idle;
        }

        public override void Enter()
        {
            guest.AssignedTableSeat.OnFoodArrival += OnFoodArrival;
            guest.AssignedTableSeat.OnFoodComing += OnFoodComing;

            guest.image.sprite = guest.actorData.sprite;
            _currentWanderTime = 0.0f;
        }

        public override void Tick()
        {
            guest.needSystem.Tick();

            if (guest.InteractableState.IsHovering)
            {
                guest.transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                guest.transform.localScale = Vector3.one;
            }

            if (guest.InteractableState.IsInteracting)
            {
                _stateMachine.ChangeState(GuestStateID.Consume);
            }

            if (!guest.needSystem.IsSatisfied() || isFoodComing)
            {
                return;
            }

            _currentWanderTime += Time.deltaTime;

            if (_currentWanderTime >= guest.wanderCheckFrequency)
            {
                float randomChance = Random.Range(0.0f, 1.0f);
                if (randomChance <= guest.wanderWhenHappyChance)
                {
                    _stateMachine.ChangeState(GuestStateID.Wander);
                }

                _currentWanderTime = 0.0f;
            }
        }

        public override void Exit()
        {
            guest.AssignedTableSeat.OnFoodArrival -= OnFoodArrival;
            guest.AssignedTableSeat.OnFoodComing -= OnFoodComing;
            isFoodComing = false;
        }

        private void OnFoodArrival(FoodRequest foodRequest)
        {
            guest.CurrentConsumable = foodRequest;
            _stateMachine.ChangeState(GuestStateID.Consume);
        }

        private void OnFoodComing()
        {
            isFoodComing = true;
        }
    }
}