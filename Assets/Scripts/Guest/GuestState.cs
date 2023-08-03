namespace Guest
{
    public enum GuestStateID
    {
        Idle,
        Moving,
        Consume,
        GetConsumable,
        MoveToSeat
    }

    public abstract class GuestState
    {
        protected GuestAI guest;
        protected GuestStateMachine _stateMachine;

        protected GuestState(GuestAI guest, GuestStateMachine stateMachine)
        {
            this.guest = guest;
            _stateMachine = stateMachine;
        }

        public abstract GuestStateID GetID();

        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }
}