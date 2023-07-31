using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCursor : MonoBehaviour
{
    private RectTransform _textRectTransform;
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private RectTransform canvasRectTransform;

    private void Awake()
    {
        _textRectTransform = transform as RectTransform;
    }

    private void Update()
    {
        // From: https://stackoverflow.com/a/75656017/10439539
        Vector2 mergedFactors = new Vector2(
            canvasRectTransform.sizeDelta.x / Screen.width,
            canvasRectTransform.sizeDelta.y / Screen.height);
        _textRectTransform.anchoredPosition = positionOffset + Mouse.current.position.value * mergedFactors;
    }
}