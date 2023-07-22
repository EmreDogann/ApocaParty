using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions
{
    public class PlateMouseInteraction : MonoBehaviour
    {
        [SerializeField] private LayerMask mask;

        private InteractableBase _hoverTarget;
        private RaycastHit2D _hit;
        private Camera _mainCamera;

        [SerializeField] private InputActionReference _interactAction;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public InteractableBase CheckForPlateInteraction()
        {
            RaycastForInteractable();

            if (!_interactAction.action.WasPressedThisFrame() && (!_hoverTarget || !_hoverTarget.IsInteractable))
            {
                return null;
            }

            return _hoverTarget;
        }

        private void RaycastForInteractable()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
                if (_mainCamera == null)
                {
                    return;
                }
            }

            // Send ray out from cursor position.
            _hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                Vector2.zero, Mathf.Infinity, mask);
            InteractableBase newTarget = null;

            if (_hit.collider)
            {
                newTarget = _hit.collider.GetComponent<InteractableBase>();
            }

            if (newTarget == _hoverTarget)
            {
                return;
            }

            _hoverTarget?.OnEndHover();
            newTarget?.OnStartHover();

            _hoverTarget = newTarget;
        }
    }
}