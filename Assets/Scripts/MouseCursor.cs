using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCursor : MonoBehaviour
{
    [SerializeField] private bool useCustomCursor;

    private void Awake()
    {
        Cursor.visible = !useCustomCursor;
        transform.gameObject.SetActive(useCustomCursor);
    }

    private void OnValidate()
    {
        Cursor.visible = !useCustomCursor;
        transform.gameObject.SetActive(useCustomCursor);
    }

    private void Update()
    {
        transform.position = Mouse.current.position.value;
    }
}