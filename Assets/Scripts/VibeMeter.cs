using System;
using System.Collections;
using Events;
using MyBox;
using UI.Components;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class VibeMeter : MonoBehaviour
{
    [SerializeField] private float winThreshold;
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup fadeOutCanvasGroup;
    [SerializeField] private BoolEventChannelSO OnGamePauseEvent;

    [ReadOnly] [SerializeField] private float _vibeMeter = 100.0f;

    public static event Action VibeCheck;
    public static Action<float> ChangeVibe;

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
        fillImage.fillAmount = _vibeMeter / 100.0f;
    }

    private void DoomsdayArrived()
    {
        if (_vibeMeter >= winThreshold)
        {
            // TODO: Win state
        }
        // TODO: Lose state

        OnGamePauseEvent?.Raise(true);
        StartCoroutine(Show());
    }

    private void ChangeVibeCallback(float vibeValue)
    {
        _vibeMeter += vibeValue;
    }

    private void DoomsdayReminder()
    {
        VibeCheck?.Invoke();
    }

    private IEnumerator Show()
    {
        yield return Fade(0.0f, 1.0f, 5.0f, Easing.QuadraticInOut.GetFunction());
        yield return new WaitForSecondsRealtime(3.0f);
        yield return Fade(1.0f, 0.0f, 5.0f, Easing.QuadraticInOut.GetFunction());
        yield return new WaitForSecondsRealtime(3.0f);
        // TODO: Open Dialogue
    }

    private IEnumerator Fade(float start, float end, float duration, Func<float, float> ease)
    {
        float current = fadeOutCanvasGroup.alpha;
        float elapsedTime = Mathf.InverseLerp(start, end, current) * duration;

        while (elapsedTime < duration)
        {
            fadeOutCanvasGroup.alpha = Mathf.Lerp(start, end, ease(elapsedTime / duration));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeOutCanvasGroup.alpha = end;
    }
}