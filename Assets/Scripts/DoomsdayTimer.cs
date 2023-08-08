using System;
using DG.Tweening;
using MyBox;
using TMPro;
using UnityEngine;

public class DoomsdayTimer : MonoBehaviour
{
    public float timeValue = 90.0f;
    public TextMeshProUGUI timeText;
    [SerializeField] private float doomsdayTimerInterval;
    [SerializeField] private bool startTimeOnAwake;

    [Separator("Scale Bounce Tweening")]
    [SerializeField] private float playFrequency = 1.0f;
    [SerializeField] private Vector2 scaleAmount;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private int strength = 10;
    [SerializeField] private float elasticity = 1.0f;

    public static event Action DoomsdayReminder;
    public static event Action DoomsdayArrived;

    private float _startingTime;
    private float _currentTime;
    private bool _canCount;

    private Tween _bounceTween;

    private void Awake()
    {
        _startingTime = timeValue;

        _bounceTween = timeText.rectTransform
            .DOPunchScale(scaleAmount, duration, strength, elasticity)
            .SetAutoKill(false)
            .Pause();

        if (startTimeOnAwake)
        {
            _canCount = true;
        }
    }

    private void OnDestroy()
    {
        _bounceTween.Kill();
    }

    private void Update()
    {
        if (!_canCount)
        {
            return;
        }

        _currentTime += Time.deltaTime;

        if (!_bounceTween.IsPlaying() && _currentTime % playFrequency < 0.025f || _currentTime % playFrequency > 0.975f)
        {
            _bounceTween.Rewind();
            _bounceTween.PlayForward();
        }

        if (_currentTime > doomsdayTimerInterval)
        {
            _currentTime = 0.0f;
            DoomsdayReminder?.Invoke();
        }

        if (timeValue > 0.0f)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            timeValue = 0.0f;
        }

        if (timeValue <= 0.0f)
        {
            _canCount = false;
            DoomsdayArrived?.Invoke();
        }

        DisplayTime(timeValue);
    }

    private void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0.0f)
        {
            timeToDisplay = 0.0f;
        }
        // else if (timeToDisplay > 0)
        // {
        //     timeToDisplay += 1;
        // }

        float minutes = Mathf.FloorToInt(timeToDisplay / 60.0f);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60.0f);

        timeText.text = string.Format("<mspace=33>{0:00}<mspace=20>:<mspace=33>{1:00}</mspace>", minutes, seconds);
    }

    public void StartTimer()
    {
        _canCount = true;
    }

    public void StopTimer()
    {
        _canCount = false;
    }

    public void ResetTimer()
    {
        timeValue = _startingTime;
        DisplayTime(timeValue);
    }
}