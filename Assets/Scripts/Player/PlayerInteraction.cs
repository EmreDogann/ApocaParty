using Events.UnityEvents;
using Interactions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private LayerMask mask;

        private InteractableBase _currentTarget;
        private RaycastHit2D _hit;
        private Camera _mainCamera;

        [SerializeField] private InputActionReference _interactAction;
        private bool _isInteractPressed;
        private bool _isInteracting;

        private bool _isPaused;
        private BoolEventListener _onGamePausedEvent;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _onGamePausedEvent = GetComponent<BoolEventListener>();
        }

        private void OnEnable()
        {
            _interactAction.action.started += OnInteractAction;
            _interactAction.action.canceled += OnInteractAction;

            _onGamePausedEvent.Response.AddListener(OnGamePause);
        }

        private void OnDisable()
        {
            _interactAction.action.started -= OnInteractAction;
            _interactAction.action.canceled -= OnInteractAction;

            _onGamePausedEvent.Response.RemoveListener(OnGamePause);
        }

        private void Update()
        {
            if (_isInteracting && _interactAction.action.WasReleasedThisFrame())
            {
                StopInteraction();
            }

            if (!_isInteractPressed)
            {
                RaycastForInteractable();
            }

            // Some house-keeping.
            if (_currentTarget != null)
            {
                if (!_currentTarget.IsInteractable)
                {
                    if (_isInteracting)
                    {
                        StopInteraction();
                    }

                    return;
                }

                if (_interactAction.action.WasPressedThisFrame())
                {
                    _isInteracting = true;

                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    mouseDelta = _mainCamera.ScreenToViewportPoint(mouseDelta);

                    _currentTarget?.OnStartInteract();
                }

                if (_isInteracting)
                {
                    if (!_currentTarget.HoldInteract || _isPaused)
                    {
                        StopInteraction();
                        return;
                    }

                    if (_currentTarget.IsHoldInteractFinished())
                    {
                        StopInteraction();
                        return;
                    }

                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    mouseDelta = _mainCamera.ScreenToViewportPoint(mouseDelta);

                    _currentTarget.OnInteract();
                }
            }
        }

        private void StopInteraction()
        {
            _isInteracting = false;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseDelta = _mainCamera.ScreenToViewportPoint(mouseDelta);

            _currentTarget?.OnEndInteract();
        }

        private void RaycastForInteractable()
        {
            // Send ray out from cursor position.
            _hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()),
                Vector2.zero, 1, mask);
            InteractableBase newTarget = null;

            if (_hit.collider)
            {
                newTarget = _hit.collider.GetComponent<InteractableBase>();
            }

            if (newTarget == _currentTarget)
            {
                return;
            }

            _currentTarget?.OnEndHover();
            newTarget?.OnStartHover();

            _currentTarget = newTarget;
        }

        private void OnInteractAction(InputAction.CallbackContext ctx)
        {
            _isInteractPressed = ctx.action.IsPressed();
        }

        private void OnGamePause(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}