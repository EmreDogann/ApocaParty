namespace Guest.States
{
    public class GuestWanderState : GuestState
    {
        private const float DistanceThreshold = 0.1f;
        public GuestWanderState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Wander;
        }

        public override void Enter() {}

        public override void Tick() {}

        public override void Exit() {}
    }
}