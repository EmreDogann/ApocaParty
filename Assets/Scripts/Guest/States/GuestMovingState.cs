namespace Guest.States
{
    public class GuestMovingState : GuestState
    {
        private const float DistanceThreshold = 0.1f;
        public GuestMovingState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Moving;
        }

        public override void Enter() {}

        public override void Tick() {}

        public override void Exit() {}
    }
}