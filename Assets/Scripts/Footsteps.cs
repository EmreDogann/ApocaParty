using Audio;
using LlamAcademy.Spring.Runtime;
using UnityEngine;

[RequireComponent(typeof(CharacterBlackboard))]
public class Footsteps : MonoBehaviour
{
    // [SerializeField] private LayerMask collisionDetectionLayerMask;
    // [SerializeField] private float collisionRadius;
    public SpringToScale scaleSpring;
    public float footstepForce = 1.0f;
    public float footstepStride = 1.5f;

    public AudioSO footstepSoundEffect;

    private CharacterBlackboard _blackboard;

    private float _currentStrideDistance;
    private float _prevPlayerStride;
    private Vector3 _prevTransformPos = Vector3.zero;

    // private RaycastHit _hit;

    private void Awake()
    {
        _blackboard = GetComponent<CharacterBlackboard>();
        _blackboard.OnStrideChange += OnStrideChange;
        _blackboard.OnStride += InteractSurface;
    }

    private void OnDestroy()
    {
        _blackboard.OnStrideChange -= OnStrideChange;
        _blackboard.OnStride -= InteractSurface;
    }

    private void LateUpdate()
    {
        HandleStride();
        _prevTransformPos = transform.position;
    }

    private void HandleStride()
    {
        _currentStrideDistance += Vector3.Magnitude(transform.position - _prevTransformPos);

        if (!_blackboard.IsMoving)
        {
            _currentStrideDistance = footstepStride - 0.2f;
        }

        if (_currentStrideDistance < footstepStride)
        {
            return;
        }

        _currentStrideDistance = 0;
        _blackboard.OnStride?.Invoke();

        footstepSoundEffect.Play();
    }

    private void InteractSurface()
    {
        // Useful for audio
        // if (Physics.SphereCast(transform.position + Vector3.up * (collisionRadius + 0.01f), collisionRadius,
        //         Vector3.down, out _hit,
        //         collisionRadius,
        //         collisionDetectionLayerMask))
        // {
        //     if (_hit.transform.TryGetComponent(out ISteppable component))
        //     {
        //         
        //     }
        // }
        scaleSpring.Nudge(new Vector3(footstepForce, footstepForce, 0.0f));
    }

    private void OnStrideChange(float newStride)
    {
        // Keep the same percentage distance to the target from the previous stride to the new stride.
        float amountOfChange = newStride / _prevPlayerStride;
        _prevPlayerStride = newStride;
        _currentStrideDistance *= amountOfChange;
    }
}