using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class ScaleUIToTextSize : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
    }

    private void Update()
    {
        Vector2 textSize = text.GetRenderedValues(true);
        textSize.x *= 1.9f;
        textSize.y *= 2.6f;
        _rectTransform.sizeDelta = textSize;
    }
}