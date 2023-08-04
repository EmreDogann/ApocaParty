using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image progressBar;
    [SerializeField] private bool hideOnAwake;
    [SerializeField] private float showFadeDuration;
    [SerializeField] private float hideFadeDuration;


    private CanvasGroup _canvasGroup;
    private bool _isActive;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (hideOnAwake)
        {
            _canvasGroup.alpha = 0.0f;
        }
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
        }
        else
        {
            _canvasGroup.DOFade(0.0f, hideFadeDuration);
        }
    }
}