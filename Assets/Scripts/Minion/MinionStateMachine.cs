using System.Collections.Generic;
using UnityEngine;

namespace Minion
{
    public class MinionStateMachine
    {
        public List<MinionState> states = new List<MinionState>();
        private MinionStateID _currentState;

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