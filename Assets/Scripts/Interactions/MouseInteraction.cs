using System;
using Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Interactions
{
    public class MouseInteraction : MonoBehaviour, IInteractionHandler
    {
        [SerializeField] private LayerMask mask;

        private InteractableBase _hoverTarget;
        private InteractableBase _activeTarget;
        private RaycastHit2D _hit;
        private Camera _mainCamera;

        [SerializeField] private InputActionReference _interactAction;
        [SerializeField] private InputActionReference _altInteractAction;
        private bool _isInteractPressed;

        private bool _isPaused;
        private BoolEventListener _onGamePausedEvent;

        public static Action<InteractableBase> OnInteract;
        public static Action<InteractableBase> OnAltInteract;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _onGamePausedEvent = GetComponent<BoolEventListener>();
        }

        private void OnEnable()
        {
            _interactAction.action.started += OnInteractAction;
            _interactAction.action.canceled += OnInteractAction;

            _altInteractAction.action.started += OnInteractAction;
            _altInteractAction.action.canceled += OnInteractAction;

            _onGamePausedEvent.Response.AddListener(OnGamePause);
        }

        private void OnDisable()
        {
            _interactAction.action.started -= OnInteractAction;
            _interactAction.action.canceled -= OnInteractAction;

            _altInteractAction.action.started -= OnInteractAction;
            _altInteractAction.action.canceled -= OnInteractAction;

            _onGamePausedEvent.Response.RemoveListener(OnGamePause);
        }

        private void Update()
        {
            if (!_isPaused)
            {
                CheckForInteraction();
            }
        }

        public InteractableBase CheckForInteraction()
        {
            if (!_activeTarget)
            {
                RaycastForInteractable();
            }

            if (!_hoverTarget || !_hoverTarget.IsInteractable)
            {
                return _activeTarget;
            }

            if (_activeTarget)
            {
                if (!_activeTarget.HoldInteract || _isPaused)
                {
                    StopInteraction();
                    return _activeTarget;
                }

                if (_activeTarget.IsHoldInteractFinished())
                {
                    StopInteraction();
                    return _activeTarget;
                }

                _activeTarget.OnInteract();
            }
            else
            {
                if (_interactAction.action.WasPressedThisFrame())
                {
                    _activeTarget = _hoverTarget;
                    _activeTarget?.OnStartInteract();
                    OnInteract?.Invoke(_activeTarget);

                    return _activeTarget;
                }

                if (_altInteractAction.action.WasPressedThisFrame())
                {
                    _activeTarget = _hoverTarget;
                    _activeTarget?.OnStartInteract();
                    OnAltInteract?.Invoke(_activeTarget);

                    return _activeTarget;
                }
            }

            return _activeTarget;
        }

        public bool WasInteractedThisFrame()
        {
            return _interactAction.action.WasPressedThisFrame() || _altInteractAction.action.WasPressedThisFrame();
        }

        private void StopInteraction()
        {
            _activeTarget?.OnEndInteract();
            _activeTarget = null;
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

            if (_hoverTarget != null && _hoverTarget.IsHoverable)
            {
                _hoverTarget.OnEndHover();
            }

            if (newTarget != null && newTarget.IsHoverable)
            {
                newTarget.OnStartHover();
            }

            _hoverTarget = newTarget;
        }

        private void OnInteractAction(InputAction.CallbackContext ctx)
        {
            if (_activeTarget && ctx.action.IsPressed())
            {
                StopInteraction();
            }
        }

        private void OnGamePause(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}