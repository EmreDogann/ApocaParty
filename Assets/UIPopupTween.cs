using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPopupTween : MonoBehaviour
{
    [SerializeField] private RectTransform hiddenPosition;
    [SerializeField] private Ease easeType = Ease.Linear;
    [SerializeField] private float animationDuration;

    private RectTransform _rectTransform;
    private Vector2 _startingPosition;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;
        _startingPosition = _rectTransform.anchoredPosition;
    }

    public void SetToHidden()
    {
        _rectTransform.anchoredPosition = hiddenPosition.anchoredPosition;
    }

    public void PopupShow()
    {
        SetToHidden();
        _rectTransform
            .DOAnchorPos(_startingPosition, animationDuration, true)
            .SetEase(easeType);
    }
}