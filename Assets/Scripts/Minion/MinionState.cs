namespace Minion
{
    public enum MinionStateID
    {
        Idle,
        Working,
        Moving,
        Slip,
        Cleanup
    }

    public abstract class MinionState
    {
        protected MinionAI minion;
        protected MinionStateMachine _stateMachine;

        protected MinionState(MinionAI minion, MinionStateMachine stateMachine)
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