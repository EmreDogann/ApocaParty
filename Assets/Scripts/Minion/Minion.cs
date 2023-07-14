using Minion.States;
using UnityEngine;
using UnityEngine.AI;

namespace Minion
{
    public enum MinionRole
    {
        Chef,
        DJ,
        EventPlanner
    }

    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent))]
    public class Minion : MonoBehaviour
    {
        public MinionStateMachine stateMachine;
        public NavMeshAgent navMeshAgent { get; private set; }

        private MinionIdleState _minionIdleState;
        private MinionTravellingState _minionTravellingState;

        public MinionRole minionRole;

        public Camera _mainCamera { get; private set; }
        public Transform marker;

        public bool showPath;
        public LineRenderer pathRenderer;

        private CharacterBlackboard _blackboard;

        private void Start()
        {
            stateMachine = new MinionStateMachine();
            _minionIdleState = new MinionIdleState(this, stateMachine);
            _minionTravellingState = new MinionTravellingState(this, stateMachine);

            stateMachine.RegisterState(_minionIdleState);
            stateMachine.RegisterState(_minionTravellingState);

            stateMachine.ChangeState(MinionStateID.Idle);

            _mainCamera = Camera.main;
            _blackboard = GetComponent<CharacterBlackboard>();

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
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

        public MinionRole GetMinionRole()
        {
            return minionRole;
        }
    }
}