using Consumable;
using UnityEngine;

namespace Guest.States
{
    public class GuestConsumeState : GuestState
    {
        private readonly float _consumeDuration = 5.0f;
        private float _currentTime;
        public GuestConsumeState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Consume;
        }

        public override void Enter()
        {
            _currentTime = 0.0f;
        }

        public override void Tick()
        {
            if (guest.CurrentConsumable != null)
            {
                guest.CurrentConsumable.GetTransform().position = guest.GetHoldingPosition().position;
            }

            _currentTime += Time.deltaTime;
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


                _stateMachine.ChangeState(GuestStateID.Idle);
            }
        }

        public override void Exit() {}
    }
}