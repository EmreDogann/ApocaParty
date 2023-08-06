namespace Guest.States
{
    public class GuestIdleState : GuestState
    {
        public GuestIdleState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Idle;
        }

        public override void Enter()
        {
            guest.InteractableState.SetInteractableActive(true);
        }

        public override void Tick()
        {
            if (guest.TutorialMode)
            {
                return;
            }

            guest.needSystem.Tick();
        }

        public override void Exit() {}
    }
}