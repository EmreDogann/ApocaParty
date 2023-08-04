using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FollowCursor : MonoBehaviour
{
    [Tooltip("The number of pixels to offset the tooltip text from the bottom-right corner of the cursor.")]
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private Canvas canvas;

    private TextMeshProUGUI _tooltipText;
    private RectTransform _canvasRectTransform;
    private RectTransform _textRectTransform;
    private RectTransform _cursorRectTransform;
    private Vector2 _cursorSize;

    private void Start()
    {
        _tooltipText = GetComponent<TextMeshProUGUI>();
        _canvasRectTransform = canvas.transform as RectTransform;
        _textRectTransform = transform as RectTransform;
        _cursorRectTransform = GameObject.FindWithTag("Cursor").GetComponent<RectTransform>();
    }

    private void Update()
    {
        // From: https://stackoverflow.com/a/75656017/10439539
        Vector2 mergedFactors = new Vector2(
            _canvasRectTransform.sizeDelta.x / Screen.width,
            _canvasRectTransform.sizeDelta.y / Screen.height);

        // For cursor rendered using SpriteRenderer.
        // Other potential fixes: https://discussions.unity.com/t/getting-a-sprites-size-in-pixels/148018/3
        // Adapted from: https://forum.unity.com/threads/corners-of-spriterenderer-at-any-position.1191871/
        // Vector2 cursorPosTopLeft = _mainCam.WorldToScreenPoint(_cursorSprite.bounds.min);
        // Vector2 cursorPosBottomRight = _mainCam.WorldToScreenPoint(_cursorSprite.bounds.max);
        // Vector2 cursorSize = cursorPosBottomRight - cursorPosTopLeft;
        // cursorSize.y *= -1;

        _cursorSize = RectTransformUtility.PixelAdjustRect(_cursorRectTransform, canvas).size *
                      _cursorRectTransform.lossyScale;

        _cursorSize.y *= -1;

        Vector2 anchoredPosition = new Vector2(0.0f, -_tooltipText.rectTransform.sizeDelta.y) + positionOffset +
                                   _cursorSize * mergedFactors + Mouse.current.position.value * mergedFactors;

        // Keep the text clamped to the screen bounds so rendered text at edges is not offscreen.
        anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, _tooltipText.margin.x,
            _canvasRectTransform.rect.width - _textRectTransform.rect.width - _tooltipText.margin.x * 2.0f);
        anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, _tooltipText.margin.y * 2.0f,
            _canvasRectTransform.rect.height - _textRectTransform.rect.height - _tooltipText.margin.y);

        _textRectTransform.anchoredPosition = anchoredPosition;
    }
}