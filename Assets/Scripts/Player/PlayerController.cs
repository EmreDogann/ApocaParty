using System.Collections;
using MyBox;
using PathCreation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Utils;

namespace Player
{
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        [Separator("Movement")]
        [SerializeField] private InputActionReference moveButton;
        [SerializeField] private float _distanceThreshold = 0.1f;
        [NavMeshSelector] [SerializeField] private int ignoreAreaCosts;

        [Separator("Path Rendering")]
        public bool showPath;
        public Transform marker;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private BezierPath.ControlMode controlMode = BezierPath.ControlMode.Automatic;
        [SerializeField] private float autoControlLength = 0.3f;
        [SerializeField] private float maxAngleError = 0.3f;
        [SerializeField] private float minVertexDistance;

        private NavMeshAgent _agent;
        private Camera _mainCamera;
        private CharacterBlackboard _blackboard;

        private float pathStartingDistance;

        private void Awake()
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
                    StartCoroutine(UpdatePath());
                }
            }

            if (Vector3.SqrMagnitude(transform.position - _agent.destination) < _distanceThreshold * _distanceThreshold)
            {
                _lineRenderer.positionCount = 0;
                marker.gameObject.SetActive(false);
            }

            if (_agent.hasPath)
            {
                Gradient c = _lineRenderer.colorGradient;
                var alphaKeys = c.alphaKeys;

                alphaKeys[0].time = 1 - GetPathRemainingDistance(_agent) / pathStartingDistance;

                c.alphaKeys = alphaKeys;
                _lineRenderer.colorGradient = c;

                marker.transform.position = _agent.destination;
            }
        }

        private IEnumerator UpdatePath()
        {
            while (_agent.pathPending)
            {
                yield return null;
            }

            marker.gameObject.SetActive(true);
            VertexPath vertexPath = GeneratePath(_agent.path.corners, false);

            if (showPath)
            {
                _lineRenderer.positionCount = vertexPath.NumPoints + 10;
                _lineRenderer.SetPositions(vertexPath.localPoints);
            }
            else
            {
                _lineRenderer.positionCount = 0;
                marker.gameObject.SetActive(false);
            }

            pathStartingDistance = GetPathRemainingDistance(_agent);
        }

        private VertexPath GeneratePath(Vector3[] points, bool closedPath)
        {
            // The control points for the path will be generated automatically
            BezierPath bezierPath = new BezierPath(points, closedPath, PathSpace.xy)
            {
                ControlPointMode = controlMode,
                AutoControlLength = autoControlLength
            };
            return new VertexPath(bezierPath, transform, maxAngleError, minVertexDistance);
        }

        private float GetPathRemainingDistance(NavMeshAgent navMeshAgent)
        {
            // if (navMeshAgent.pathPending ||
            //     navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
            //     navMeshAgent.path.corners.Length == 0)
            // {
            //     return -1f;
            // }

            float distance = 0.0f;
            for (int i = 0; i <= navMeshAgent.path.corners.Length - 2; ++i)
            {
                distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
            }

            // Debug.Log(distance);
            return distance;
        }
    }
}