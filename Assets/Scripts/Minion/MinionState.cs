namespace Minion
{
    public enum MinionStateID
    {
        Idle,
        Working,
        Travelling
    }

    public abstract class MinionState
    {
        protected Minion minion;
        protected MinionStateMachine _stateMachine;

        public MinionState(Minion minion, MinionStateMachine stateMachine)
        {
            this.minion = minion;
            _stateMachine = stateMachine;
        }

        public abstract MinionStateID GetID();

        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }
}