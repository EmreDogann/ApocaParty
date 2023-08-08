using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private bool useCustomCursor;

    private static Action<bool> ChangeCursor;

    public static void CursorActive(bool isActive)
    {
        ChangeCursor?.Invoke(isActive);
    }

    private void Awake()
    {
        Cursor.visible = !useCustomCursor;
        transform.gameObject.SetActive(useCustomCursor);

        ChangeCursor += ChangeCursorCallback;
    }

    private void OnValidate()
    {
        Cursor.visible = !useCustomCursor;
        transform.gameObject.SetActive(useCustomCursor);
    }

    private void OnDestroy()
    {
        ChangeCursor -= ChangeCursorCallback;
    }

    private void ChangeCursorCallback(bool isActive)
    {
        transform.gameObject.SetActive(isActive);
    }

    private void Update()
    {
        transform.position = Mouse.current.position.value;
    }
}