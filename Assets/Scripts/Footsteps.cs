using Audio;
using LlamAcademy.Spring.Runtime;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Footsteps : MonoBehaviour
{
    public SpringToScale scaleSpring;
    public float footstepForce = 1.0f;
    public float footstepStride = 1.5f;

    public AudioSO footstepSoundEffect;

    private NavMeshAgent _agent;

    private float _currentStrideDistance;
    private float _prevPlayerStride;
    private Vector3 _prevTransformPos = Vector3.zero;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void LateUpdate()
    {
        HandleStride();
        _prevTransformPos = transform.position;
    }

    private void HandleStride()
    {
        _currentStrideDistance += Vector3.Magnitude(transform.position - _prevTransformPos);

        if (!_agent.hasPath)
        {
            _currentStrideDistance = footstepStride - 0.2f;
        }

        if (_currentStrideDistance < footstepStride)
        {
            return;
        }

        _currentStrideDistance = 0;
        InteractSurface();

        footstepSoundEffect.Play(transform.position);
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
}