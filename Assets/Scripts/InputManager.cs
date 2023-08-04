using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public enum InputActionMapType
{
    Empty,
    Player,
    PlayerTutorial,
    MinionTutorial,
    UI
}

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInput _playerInput;

    public bool InteractPressed { get; private set; }
    public bool InteractReleased { get; private set; }
    public bool InteractHeld { get; private set; }
    public bool InteractAltPressed { get; private set; }
    public bool InteractAltReleased { get; private set; }
    public bool InteractAltHeld { get; private set; }

    private InputAction _interactAction;
    private InputAction _interactAltAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _playerInput = GetComponent<PlayerInput>();

        SetupActions();
    }

    private void OnEnable()
    {
        ActionMapSwitcher.SwitchActionMap += SwitchActionMap;
    }

    private void OnDisable()
    {
        ActionMapSwitcher.SwitchActionMap -= SwitchActionMap;
    }

    private void Update()
    {
        if (_interactAction != null)
        {
            InteractPressed = _interactAction.WasPressedThisFrame();
            InteractReleased = _interactAction.WasReleasedThisFrame();
            InteractHeld = _interactAction.IsPressed();
        }

        if (_interactAltAction != null)
        {
            InteractAltPressed = _interactAltAction.WasPressedThisFrame();
            InteractAltReleased = _interactAltAction.WasReleasedThisFrame();
            InteractAltHeld = _interactAltAction.IsPressed();
        }
    }

    private void SwitchActionMap(string actionMapName)
    {
        _playerInput.SwitchCurrentActionMap(actionMapName);
        SetupActions();
    }

    private void SetupActions()
    {
        _interactAction = _playerInput.currentActionMap.FindAction("Interact");
        _interactAltAction = _playerInput.currentActionMap.FindAction("InteractAlt");
    }
}