using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerBlackboard))]
    public class PlayerController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private Camera _mainCamera;
        public Transform marker;

        public bool showPath = false;
        public LineRenderer pathRenderer;
        private PlayerBlackboard _blackboard;
        void Start()
        {
            _blackboard = GetComponent<PlayerBlackboard>();
            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        void Update()
        {
            _blackboard.IsMoving = _agent.hasPath;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _agent.destination = _mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
                marker.transform.position = _agent.destination;
                marker.gameObject.SetActive(true);
            }

            if (Vector3.Distance(transform.position, marker.position) < 0.1f)
            {
                marker.gameObject.SetActive(false);
            }

            if (showPath)
            {
                var path = _agent.path;
                pathRenderer.positionCount = path.corners.Length;
                pathRenderer.SetPositions(path.corners);
            }
        }
    }
}
