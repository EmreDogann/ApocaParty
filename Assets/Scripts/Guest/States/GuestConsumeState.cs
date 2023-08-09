using Consumable;
using UnityEngine;

namespace Guest.States
{
    public class GuestConsumeState : GuestState
    {
        private readonly float _consumeDuration = 3.0f;
        private float _currentTime;
        public GuestConsumeState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Consume;
        }

        public override void Enter()
        {
            _currentTime = 0.0f;
            guest.consumeProgressBar.SetProgressBarActive(true);
            guest.InteractableState.SetInteractableActive(false);
        }

        public override void Tick()
        {
            if (guest.CurrentConsumable != null)
            {
                guest.CurrentConsumable.GetTransform().position = guest.GetHoldingTransform().position;
            }

            _currentTime += Time.deltaTime;
            guest.consumeProgressBar.SetProgressBarPercentage(Mathf.Clamp01(_currentTime / _consumeDuration));
            if (_currentTime >= _consumeDuration)
            {
                // Consume drink
                if (guest.CurrentConsumable != null)
                {
                    ConsumedData consumedData = guest.CurrentConsumable.Consume();
                    guest.needSystem.TryFulfillNeed(consumedData.needType, consumedData.needMetrics,
                        consumedData.moodPoints);
                    guest.CurrentConsumable = null;
                }

                // Consume food
                if (guest.AssignedTableSeat.IsFoodAvailable())
                {
                    ConsumedData consumedData = guest.AssignedTableSeat.GetFood().Consume();
                    guest.needSystem.TryFulfillNeed(consumedData.needType, consumedData.needMetrics,
                        consumedData.moodPoints);
                }

                _stateMachine.ChangeState(GuestStateID.Idle);
            }
        }

        public override void Exit()
        {
            guest.consumeProgressBar.SetProgressBarActive(false);
            guest.InteractableState.SetInteractableActive(true);
        }
    }
}