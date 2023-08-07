using System;
using DG.Tweening;
using Events;
using MyBox;
using UI.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VibeMeter : MonoBehaviour
{
    [Separator("Threshold")]
    [SerializeField] private Image thresholdImage;
    [SerializeField] private Image thresholdFill;

    [Separator("Vibe Meter")]
    [Range(0.0f, 100.0f)] [SerializeField] private float winThreshold;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color dangerFillColor;

    [SerializeField] private BoolEventChannelSO OnGamePauseEvent;

    [Range(0.0f, 100.0f)] [SerializeField] private float _vibeMeter = 100.0f;

    public static event Action VibeCheck;
    public static Action<float> ChangeVibe;

    public UnityEvent onGameWin;
    public UnityEvent onGameLose;

    private Tween _vibeChangeTween;
    private Tween _dangerColorTween;

    private void Awake()
    {
        if (thresholdImage != null)
        {
            Vector2 anchoredPosition = thresholdImage.rectTransform.anchoredPosition;
            anchoredPosition.y = winThreshold / 100.0f * fillImage.rectTransform.sizeDelta.y -
                                 fillImage.rectTransform.sizeDelta.y / 2.0f;

            thresholdImage.rectTransform.anchoredPosition = anchoredPosition;
        }

        if (thresholdFill != null)
        {
            thresholdFill.fillAmount = winThreshold / 100.0f;
        }
        
        _dangerColorTween = fillImage
            .DOColor(dangerFillColor, 0.8f)
            .SetEase(Ease.InOutFlash)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnValidate()
    {
        if (thresholdImage != null)
        {
            Vector2 anchoredPosition = thresholdImage.rectTransform.anchoredPosition;
            anchoredPosition.y = winThreshold / 100.0f * fillImage.rectTransform.sizeDelta.y -
                                 fillImage.rectTransform.sizeDelta.y / 2.0f;

            thresholdImage.rectTransform.anchoredPosition = anchoredPosition;
        }

        if (thresholdFill != null)
        {
            thresholdFill.fillAmount = winThreshold / 100.0f;
        }

        fillImage.fillAmount = _vibeMeter / 100.0f;
    }

    private void OnEnable()
    {
        DoomsdayTimer.DoomsdayReminder += DoomsdayReminder;
        DoomsdayTimer.DoomsdayArrived += DoomsdayArrived;
        ChangeVibe += ChangeVibeCallback;
    }

    private void OnDisable()
    {
        DoomsdayTimer.DoomsdayReminder -= DoomsdayReminder;
        DoomsdayTimer.DoomsdayArrived -= DoomsdayArrived;
        ChangeVibe -= ChangeVibeCallback;
    }

    private void Update()
    {
        // fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, _vibeMeter / 100.0f, 0.3f * Time.deltaTime);
    }

    private void DoomsdayArrived()
    {
        if (_vibeMeter >= winThreshold)
        {
            onGameWin?.Invoke();
        }
        else
        {
            onGameLose?.Invoke();
        }

        OnGamePauseEvent?.Raise(true);
    }

    public void SetVibe(float vibeValue)
    {
        _vibeMeter = Mathf.Clamp(vibeValue, 0.0f, 100.0f);

        _vibeChangeTween.Kill(true);
        _vibeChangeTween = fillImage
            .DOFillAmount(_vibeMeter / 100.0f, 0.7f)
            .SetEase(Ease.OutBack, 0.8f)
            .OnUpdate(() =>
            {
                if (_dangerColorTween.IsPlaying())
                {
                    if (_vibeMeter >= winThreshold)
                    {
                        _dangerColorTween.Rewind();
                    }

                    return;
                }

                if (_vibeMeter < winThreshold)
                {
                    _dangerColorTween.PlayForward();
                }
            });
    }

    private void ChangeVibeCallback(float vibeValueToAdd)
    {
        _vibeMeter += vibeValueToAdd;
        SetVibe(_vibeMeter);
    }

    private void DoomsdayReminder()
    {
        // VibeCheck?.Invoke();
    }
}