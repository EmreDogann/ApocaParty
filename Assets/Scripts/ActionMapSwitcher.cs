using System;
using UnityEngine;

public class ActionMapSwitcher : MonoBehaviour
{
    [SerializeField] private InputActionMapType actionMap;

    public static event Action<string> SwitchActionMap;

    public void ChangeActionMap()
    {
        SwitchActionMap?.Invoke(actionMap.ToString());
    }
}