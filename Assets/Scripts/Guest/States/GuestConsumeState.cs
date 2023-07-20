using UnityEngine;

namespace Guest.States
{
    public class GuestConsumeState : GuestState
    {
        public GuestConsumeState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Consume;
        }

        public override void Enter()
        {
            guest.currentRequest.StartRequest();
        }

        public override void Tick()
        {
            guest.currentRequest.UpdateRequest(Time.deltaTime);
            if (guest.currentRequest.IsRequestCompleted())
            {
                _stateMachine.ChangeState(GuestStateID.Idle);
            }
        }

        public override void Exit() {}
    }
}