using System;

namespace Guest
{
    public class GuestStateMachine
    {
        private readonly GuestState[] _states;
        private GuestStateID _currentState;

        public GuestStateMachine()
        {
            int numStates = Enum.GetNames(typeof(GuestStateID)).Length;
            _states = new GuestState[numStates];
        }

        public void RegisterState(GuestState state)
        {
            int index = (int)state.GetID();
            _states[index] = state;
        }

        public GuestState GetState(GuestStateID stateID)
        {
            int index = (int)stateID;
            return _states[index];
        }

        public GuestState GetCurrentState()
        {
            return GetState(_currentState);
        }

        public void UpdateState()
        {
            GetState(_currentState)?.Tick();
        }

        public void ChangeState(GuestStateID newState)
        {
            GetState(_currentState)?.Exit();
            _currentState = newState;
            GetState(_currentState)?.Enter();
        }
    }
}