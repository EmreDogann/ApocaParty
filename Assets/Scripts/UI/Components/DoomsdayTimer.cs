using System;
using TMPro;
using UnityEngine;

namespace UI.Components
{
    public class DoomsdayTimer : MonoBehaviour
    {
        public float timeValue = 90.0f;
        public TextMeshProUGUI timeText;
        [SerializeField] private float doomsdayTimerInterval;

        public static event Action DoomsdayReminder;
        public static event Action DoomsdayArrived;

        private float _currentTime;
        private bool canCount;

        private void Update()
        {
            if (!canCount)
            {
                return;
            }

            _currentTime += Time.deltaTime;

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
                canCount = false;
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

            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        public void StartTimer()
        {
            canCount = true;
        }

        public void StopTimer()
        {
            canCount = false;
        }
    }
}