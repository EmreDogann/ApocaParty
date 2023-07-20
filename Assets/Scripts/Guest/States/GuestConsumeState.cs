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
            _currentTime += Time.deltaTime;
            if (_currentTime >= _consumeDuration)
            {
                ConsumedData consumedData = guest.HoldingConsumable.Consume();
                guest.needSystem.TryFulfillNeed(consumedData.needType, consumedData.needMetrics,
                    consumedData.moodPoints);
                guest.HoldingConsumable = null;

                _stateMachine.ChangeState(GuestStateID.Idle);
            }
        }

        public override void Exit() {}
    }
}