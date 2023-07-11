using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Camera _mainCamera;
    public Transform marker;

    public bool showPath = false;
    public LineRenderer pathRenderer;
    void Start()
    {
        _mainCamera = Camera.main;
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update()
    {
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
            pathRenderer.positionCount = _agent.path.corners.Length;
            pathRenderer.SetPositions(_agent.path.corners);
        }
    }
}
