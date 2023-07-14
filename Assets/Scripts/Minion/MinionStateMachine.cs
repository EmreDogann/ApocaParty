using System;
using UnityEngine;

namespace Minion
{
    public class MinionStateMachine
    {
        public MinionState[] states;
        private MinionStateID _currentState;

        public MinionStateMachine()
        {
            int numStates = Enum.GetNames(typeof(MinionStateID)).Length;
            states = new MinionState[numStates];
        }

        public void RegisterState(MinionState state)
        {
            int index = (int)state.GetID();
            states[index] = state;
        }

        public MinionState GetState(MinionStateID stateID)
        {
            int index = (int)stateID;
            return states[index];
        }

        public void UpdateState()
        {
            GetState(_currentState)?.Tick();
        }

        public void ChangeState(MinionStateID newState)
        {
            GetState(_currentState)?.Exit();
            _currentState = newState;
            GetState(_currentState)?.Enter();

            Debug.Log("Current State: " + newState);
        }
    }
}