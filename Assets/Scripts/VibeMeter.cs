using System;
using Audio;
using DG.Tweening;
using Events;
using MyBox;
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

    [Separator("Audio")]
    [SerializeField] private AudioSO increaseAudio;
    [SerializeField] private AudioSO decreaseAudio;

    [Separator("Events")]
    [SerializeField] private BoolEventChannelSO OnGamePauseEvent;
    [SerializeField] private BoolEventChannelSO OnGameEnd;

    [Range(0.0f, 100.0f)] [SerializeField] private float _vibeMeter = 100.0f;

    public static Action<float, bool> ChangeVibe;

    public UnityEvent onGameWin;
    public UnityEvent onGameLose;

    private Tween _vibeChangeTween;
    private Tween _dangerColorTween;

    private bool _vibeChangeTweenActive;

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
            .SetLoops(-1, LoopType.Yoyo)
            .Pause();
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

    private void OnDestroy()
    {
        _dangerColorTween.Kill();
    }

    private void DoomsdayArrived()
    {
        if (_vibeMeter >= winThreshold)
        {
            onGameWin?.Invoke();
            OnGameEnd?.Raise(true);
        }
        else
        {
            onGameLose?.Invoke();
            OnGameEnd?.Raise(false);
        }

        OnGamePauseEvent?.Raise(true);
    }

    public void SetVibe(float vibeValue)
    {
        _vibeMeter = Mathf.Clamp(vibeValue, 0.0f, 100.0f);

        _vibeChangeTween.Kill();
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
            })
            .OnComplete(() => { _vibeChangeTweenActive = false; });
    }

    private void ChangeVibeCallback(float valueToAdd, bool playAudio)
    {
        _vibeMeter += valueToAdd;
        SetVibe(_vibeMeter);

        if (playAudio && !_vibeChangeTweenActive)
        {
            _vibeChangeTweenActive = true;
            if (Mathf.Sign(valueToAdd) > 0.0f)
            {
                increaseAudio.Play();
            }
            else
            {
                decreaseAudio.Play();
            }
        }
    }

    public void Tutorial_PlayVibeIncreaseSound()
    {
        increaseAudio.Play();
    }

    public void Tutorial_PlayVibeDecreaseSound()
    {
        decreaseAudio.Play();
    }

    private void DoomsdayReminder()
    {
        // VibeCheck?.Invoke();
    }
}