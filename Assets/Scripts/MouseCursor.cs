using System;
using Audio;
using MyBox;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private CursorLockMode cursorLockMode;

    [SerializeField] private bool useCustomCursor;
    [SerializeField] private Image customCursor;

    [Separator("Butter Fingers Mode")]
    [SerializeField] private InteractableTooltip tooltip;
    [SerializeField] private string tooltipMessage;

    [SerializeField] private SpriteRenderer spriteMouseCursor;
    [SerializeField] private Image inputBlocker;
    [Layer] [SerializeField] private int clickCollisionLayer;

    [Space]
    [Range(0.0f, 0.3f)] [SerializeField] private float mouseFollowerSmoothTime;
    [SerializeField] private float speedThreshold;
    [SerializeField] private float speedSensitivity = 1.0f;
    [SerializeField] private float physicsCursorForceSensitivity = 1.0f;
    [ReadOnly] [SerializeField] private float currentMouseSpeed; // FOr Debugging

    [SerializeField] private AudioSO butterFingersSound;

    private bool _butterFingersMode;
    private Vector2 _mouseFollower;
    private Vector2 _currentMouseVelocity;
    private Rigidbody2D _spriteMouseCursorRB;
    private Camera _main;

    private RaycastHit2D _currentTarget;
    private bool _isButterFingering;

    private static Action<bool> _changeCursor;

    public static void CursorActive(bool isActive)
    {
        _changeCursor?.Invoke(isActive);
    }

    private void Awake()
    {
        _main = Camera.main;

        Cursor.lockState = cursorLockMode;
        Cursor.visible = !useCustomCursor;
        if (customCursor != null)
        {
            customCursor.enabled = useCustomCursor;
        }

        _changeCursor += ChangeCursorCallback;

        if (spriteMouseCursor)
        {
            spriteMouseCursor.enabled = false;
            _spriteMouseCursorRB = spriteMouseCursor.GetComponent<Rigidbody2D>();
            _spriteMouseCursorRB.isKinematic = true;
        }

        inputBlocker.enabled = false;
    }

    private void OnValidate()
    {
        Cursor.lockState = cursorLockMode;
        Cursor.visible = !useCustomCursor;
        if (customCursor != null)
        {
            customCursor.enabled = useCustomCursor;
        }
    }

    private void OnDestroy()
    {
        _changeCursor -= ChangeCursorCallback;
    }

    private void ChangeCursorCallback(bool isActive)
    {
        customCursor.enabled = isActive;
    }

    public void ButterFingersMode(bool modeActive)
    {
        _butterFingersMode = modeActive;
        if (_butterFingersMode)
        {
            if (CheckForCamera())
            {
                _mouseFollower = _main.ScreenToWorldPoint(customCursor.transform.position);
                _currentMouseVelocity = Vector2.zero;
            }
        }
        else
        {
            tooltip.SetTooltip(string.Empty);
            TogglePhysicsCursor(false);
            customCursor.rectTransform.rotation = Quaternion.identity;
        }
    }

    private void TogglePhysicsCursor(bool isActive)
    {
        _isButterFingering = isActive;

        Cursor.visible = isActive;
        inputBlocker.enabled = isActive;

        _spriteMouseCursorRB.isKinematic = !isActive;
        _spriteMouseCursorRB.velocity = Vector2.zero;
        _spriteMouseCursorRB.angularVelocity = 0.0f;
    }

    private bool CheckForCamera()
    {
        if (_main == null)
        {
            _main = Camera.main;
            if (_main == null)
            {
                return false;
            }
        }

        return true;
    }

    private void Update()
    {
        if (!_isButterFingering)
        {
            customCursor.rectTransform.position = Mouse.current.position.value;
        }

        if (_butterFingersMode)
        {
            if (!CheckForCamera())
            {
                return;
            }

            Vector2 target = _main.ScreenToWorldPoint(customCursor.transform.position);
            _mouseFollower = Vector2.SmoothDamp(_mouseFollower, target, ref _currentMouseVelocity,
                mouseFollowerSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

#if UNITY_EDITOR
            // For debugging
            currentMouseSpeed = (_currentMouseVelocity * speedSensitivity).magnitude;
#endif

            if (!_isButterFingering && Time.timeScale != 0.0f &&
                (_currentMouseVelocity * speedSensitivity).sqrMagnitude >= speedThreshold * speedThreshold)
            {
                TogglePhysicsCursor(true);
                butterFingersSound.Play2D();

                spriteMouseCursor.transform.position = target;
                spriteMouseCursor.transform.rotation = Quaternion.identity;

                _spriteMouseCursorRB.AddForce(
                    _currentMouseVelocity * physicsCursorForceSensitivity,
                    ForceMode2D.Impulse);

                _spriteMouseCursorRB.AddTorque(1.0f, ForceMode2D.Impulse);
            }

            if (_isButterFingering)
            {
                customCursor.rectTransform.position =
                    _main.WorldToScreenPoint(_spriteMouseCursorRB.transform.position);
                customCursor.rectTransform.rotation = _spriteMouseCursorRB.transform.rotation;

                RaycastHit2D hit =
                    Physics2D.Raycast(_main.ScreenToWorldPoint(Mouse.current.position.value), Vector2.zero,
                        Mathf.Infinity, 1 << clickCollisionLayer);

                if (hit.collider != _currentTarget.collider)
                {
                    tooltip.SetTooltip(hit.collider != null ? tooltipMessage : string.Empty);
                    _currentTarget = hit;
                }

                if (_currentTarget.collider != null)
                {
                    if (InputManager.Instance.InteractPressed || InputManager.Instance.InteractAltPressed)
                    {
                        tooltip.SetTooltip(string.Empty);

                        TogglePhysicsCursor(false);
                        customCursor.rectTransform.rotation = Quaternion.identity;
                    }
                }
            }
        }
    }
}