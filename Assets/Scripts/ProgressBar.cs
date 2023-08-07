using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private bool hideOnAwake;
    [SerializeField] private float showFadeDuration;
    [SerializeField] private float hideFadeDuration;

    [Separator("Effects")]
    [SerializeField] private bool pulsatingEffect;

    private CanvasGroup _canvasGroup;
    private bool _isActive;
    private Sequence _pulsateEffect;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (hideOnAwake)
        {
            _canvasGroup.alpha = 0.0f;
        }

        _pulsateEffect = DOTween.Sequence().SetLoops(-1, LoopType.Yoyo);
        _pulsateEffect
            .PrependInterval(0.7f)
            .Append(progressBar.rectTransform.DOScale(progressBar.rectTransform.localScale * 1.05f, 0.6f)
                .SetEase(Ease.InOutQuad))
            .Pause();
    }

    public void SetProgressBarPercentage(float percentage)
    {
        progressBar.fillAmount = Mathf.Clamp01(percentage);
    }

    public void SetProgressBarActive(bool isActive)
    {
        _isActive = isActive;
        if (_isActive)
        {
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1.0f, showFadeDuration);

            if (pulsatingEffect)
            {
                _pulsateEffect.PlayForward();
            }
        }
        else
        {
            _canvasGroup.DOFade(0.0f, hideFadeDuration);
            if (pulsatingEffect)
            {
                _pulsateEffect.SmoothRewind();
            }
        }
    }
}