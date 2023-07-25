using System;
using Events;
using MyBox;
using UI.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VibeMeter : MonoBehaviour
{
    [SerializeField] private float winThreshold;
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup fadeOutCanvasGroup;
    [SerializeField] private BoolEventChannelSO OnGamePauseEvent;

    [ReadOnly] [SerializeField] private float _vibeMeter = 100.0f;

    public static event Action VibeCheck;
    public static Action<float> ChangeVibe;

    public UnityEvent onGameWin;
    public UnityEvent onGameLose;

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
            onGameWin?.Invoke();
        }

        onGameLose?.Invoke();
        OnGamePauseEvent?.Raise(true);
    }

    private void ChangeVibeCallback(float vibeValue)
    {
        _vibeMeter += vibeValue;
    }

    private void DoomsdayReminder()
    {
        VibeCheck?.Invoke();
    }
}