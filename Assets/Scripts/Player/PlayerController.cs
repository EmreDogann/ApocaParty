using Unity.VisualScripting;
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
            if (Time.timeScale == 0.0f)
            {
                return;
            }
            
            _blackboard.IsMoving = _agent.hasPath;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _agent.SetDestination(_mainCamera.ScreenToWorldPoint(Mouse.current.position.value));
                marker.transform.position = _agent.destination;
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
                if (_agent.hasPath) marker.gameObject.SetActive(true);
            }
            else
            {
                pathRenderer.positionCount = 0;
                marker.gameObject.SetActive(false);
            }
        }
    }
}
