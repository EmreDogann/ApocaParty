using System.Collections;
using LlamAcademy.Spring.Runtime;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerBlackboard))]
    public class PlayerFootsteps : MonoBehaviour
    {
        // [SerializeField] private LayerMask collisionDetectionLayerMask;
        // [SerializeField] private float collisionRadius;
        public SpringToScale scaleSpring;
        public float footstepForce = 1.0f;
        public float footstepStride = 1.5f;
        public float currentStrideDistance = 0.0f;
        
        private PlayerBlackboard _blackboard;

        private float _prevPlayerStride;
        private Vector3 _prevTransformPos = Vector3.zero;

        // private RaycastHit _hit;
        
        private void Awake()
        {
            _blackboard = GetComponent<PlayerBlackboard>();
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
            currentStrideDistance += Vector3.Magnitude(transform.position - _prevTransformPos);

            if (!_blackboard.IsMoving)
            {
                currentStrideDistance = footstepStride - 0.2f;
            }

            if (currentStrideDistance < footstepStride)
            {
                return;
            }
            
            currentStrideDistance = 0;
            _blackboard.OnStride?.Invoke();
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
            currentStrideDistance *= amountOfChange;
        }
    }
}