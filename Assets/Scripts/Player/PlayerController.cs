using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveButton;
        [SerializeField] private float _distanceThreshold = 0.1f;
        [NavMeshSelector] [SerializeField] private int ignoreAreaCosts;

        public bool showPath;
        public Transform marker;
        public LineRenderer pathRenderer;

        private NavMeshAgent _agent;
        private Camera _mainCamera;
        private CharacterBlackboard _blackboard;

        private void Start()
        {
            _blackboard = GetComponent<CharacterBlackboard>();
            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.SetAreaCost(ignoreAreaCosts, 1.0f);
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            _blackboard.IsMoving = _agent.hasPath;

            if (moveButton.action.WasPressedThisFrame())
            {
                Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
                mouseWorldPosition.z = 0;
                _agent.SetDestination(mouseWorldPosition);
                marker.transform.position = _agent.destination;

                if (showPath)
                {
                    marker.gameObject.SetActive(true);
                }
            }

            if (Vector3.SqrMagnitude(transform.position - _agent.destination) < _distanceThreshold * _distanceThreshold)
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