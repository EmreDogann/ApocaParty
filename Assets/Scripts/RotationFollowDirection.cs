using LlamAcademy.Spring.Runtime;
using UnityEngine;
using Utils;

[RequireComponent(typeof(SpringToRotation))]
public class RotationFollowDirection : MonoBehaviour
{
    private SpringToRotation _rotationSpring;
    [SerializeField] private float velocityThreshold = 0.1f;

    [OrthogonalUnitVector3] [SerializeField] private Vector3 flipAxis;

    private Vector3 _prevPosition;
    private int _direction = 1;

    private void Awake()
    {
        _rotationSpring = GetComponent<SpringToRotation>();
    }

    private void Update()
    {
        Vector3 currentVelocity = (transform.position - _prevPosition) / Time.deltaTime;

        _prevPosition = transform.position;

        if (currentVelocity.sqrMagnitude <= velocityThreshold * velocityThreshold)
        {
            return;
        }

        int currDir = (int)Mathf.Sign(Vector3.Dot(currentVelocity.normalized, Vector3.right));
        if (currDir != _direction)
        {
            _direction = currDir;
            FlipDirection();
        }
    }

    private void FlipDirection()
    {
        _rotationSpring.SpringTo((_direction == 1 ? 0.0f : 180.0f) * flipAxis);
    }
}