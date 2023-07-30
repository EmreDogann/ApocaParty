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
    public bool InteractHeld { get; private set; }
    public bool InteractAltPressed { get; private set; }
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

        _interactAction = _playerInput.currentActionMap["Interact"];
        _interactAltAction = _playerInput.currentActionMap["InteractAlt"];
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
        InteractPressed = _interactAction.WasPressedThisFrame();
        InteractHeld = _interactAction.IsPressed();
        InteractAltPressed = _interactAltAction.WasPressedThisFrame();
        InteractAltHeld = _interactAltAction.IsPressed();
    }

    private void SwitchActionMap(string actionMapName)
    {
        _playerInput.SwitchCurrentActionMap(actionMapName);
    }
}