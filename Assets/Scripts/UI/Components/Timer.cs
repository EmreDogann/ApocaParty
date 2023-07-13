using TMPro;
using UnityEngine;

namespace UI.Components
{
    public class Timer : MonoBehaviour
    {
        public float timeValue = 90.0f;
        public TextMeshProUGUI timeText;

        private void Update()
        {
            if (timeValue > 0.0f)
            {
                timeValue -= Time.deltaTime;
            }
            else
            {
                timeValue = 0.0f;
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
    }
}