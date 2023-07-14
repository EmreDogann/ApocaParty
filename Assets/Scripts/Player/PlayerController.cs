using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Camera _mainCamera;
        public Transform marker;

        public bool showPath;
        public LineRenderer pathRenderer;
        private CharacterBlackboard _blackboard;

        private void Start()
        {
            _blackboard = GetComponent<CharacterBlackboard>();
            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            _blackboard.IsMoving = _agent.hasPath;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _agent.SetDestination(_mainCamera.ScreenToWorldPoint(Mouse.current.position.value));
                marker.transform.position = _agent.destination;

                if (showPath)
                {
                    marker.gameObject.SetActive(true);
                }
            }

            if (Vector3.Distance(transform.position, _agent.destination) < 0.1f)
            {
                marker.gameObject.SetActive(false);
            }

            if (_agent.hasPath)
            {
                marker.transform.position = _agent.destination;
            }

            if (showPath)
            {
                NavMeshPath path = _agent.path;
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