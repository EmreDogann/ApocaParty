using Interactions.Interactables;
using Minion.States;
using UnityEngine;
using UnityEngine.AI;

namespace Minion
{
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent), typeof(MinionInteractable))]
    public class MinionAI : MonoBehaviour
    {
        public MinionStateMachine stateMachine;
        public NavMeshAgent navMeshAgent { get; private set; }
        public MinionInteractable InteractableState { get; private set; }
        public bool showPath;
        public Transform marker;
        public LineRenderer pathRenderer;

        public Camera _mainCamera { get; private set; }

        private MinionIdleState _minionIdleState;
        private MinionAssignmentState _minionAssignmentState;
        private MinionWorkingState _minionWorkingState;
        private CharacterBlackboard _blackboard;

        private void Start()
        {
            _mainCamera = Camera.main;
            _blackboard = GetComponent<CharacterBlackboard>();
            InteractableState = GetComponent<MinionInteractable>();

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;

            stateMachine = new MinionStateMachine();
            _minionIdleState = new MinionIdleState(this, stateMachine);
            _minionAssignmentState = new MinionAssignmentState(this, stateMachine);
            _minionWorkingState = new MinionWorkingState(this, stateMachine);

            stateMachine.RegisterState(_minionIdleState);
            stateMachine.RegisterState(_minionAssignmentState);
            stateMachine.RegisterState(_minionWorkingState);

            // Initial state
            stateMachine.ChangeState(MinionStateID.Idle);
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            stateMachine.UpdateState();

            _blackboard.IsMoving = navMeshAgent.hasPath;

            if (showPath)
            {
                NavMeshPath path = navMeshAgent.path;
                pathRenderer.positionCount = path.corners.Length;
                pathRenderer.SetPositions(path.corners);
            }
            else
            {
                pathRenderer.positionCount = 0;
                marker.gameObject.SetActive(false);
            }
        }
    }
}