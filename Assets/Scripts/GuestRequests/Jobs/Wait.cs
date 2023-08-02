using UnityEngine;

namespace GuestRequests.Jobs
{
    public class Wait : Job
    {
        public float Duration = 1.0f;

        public override float GetProgressPercentage()
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration()
        {
            return Duration;
        }
    }
}