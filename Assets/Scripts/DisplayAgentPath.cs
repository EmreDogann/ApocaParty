using System.Collections;
using MyBox;
using PathCreation;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DisplayAgentPath : MonoBehaviour
{
    [Separator("Path Rendering")]
    public bool showPath;
    public Transform marker;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float pathUpdateRate = 0.016f;
    [SerializeField] private float speedOffset = 0.05f;

    [Separator("Bezier Curve Settings")]
    [SerializeField] private BezierPath.ControlMode controlMode = BezierPath.ControlMode.Automatic;
    [SerializeField] private float autoControlLength = 0.3f;
    [SerializeField] private float maxAngleError = 0.3f;
    [SerializeField] private float minVertexDistance;

    private NavMeshAgent _agent;

    private float _currentTime;
    private bool _isPathDisplayed;

    private readonly int SpeedProp = Shader.PropertyToID("_Speed");

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Time.timeScale == 0.0f)
        {
            return;
        }

        if (!_isPathDisplayed)
        {
            return;
        }

        if (_agent.remainingDistance >= 0.01f)
        {
            marker.transform.position = _agent.destination;

            if (_currentTime < pathUpdateRate)
            {
                _currentTime += Time.deltaTime;
                return;
            }

            StartCoroutine(UpdatePath());
        }
    }

    public void DisplayPath()
    {
        if (showPath)
        {
            StartCoroutine(UpdatePath());
        }
    }

    public void HidePath()
    {
        _currentTime = 0.0f;
        _isPathDisplayed = false;
        _lineRenderer.positionCount = 0;

        marker.gameObject.SetActive(false);
    }

    private IEnumerator UpdatePath()
    {
        _currentTime = 0.0f;
        while (_agent.pathPending)
        {
            yield return null;
        }

        if (_agent.path.corners.Length < 2)
        {
            yield break;
        }

        _isPathDisplayed = true;

        marker.transform.position = _agent.destination;
        marker.gameObject.SetActive(true);
        VertexPath vertexPath = GeneratePath(_agent.path.corners, false);

        _lineRenderer.positionCount = vertexPath.NumPoints;
        _lineRenderer.SetPositions(vertexPath.localPoints);

        _lineRenderer.material.SetFloat(SpeedProp, -Mathf.Abs(_agent.desiredVelocity.magnitude) - speedOffset);
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
}