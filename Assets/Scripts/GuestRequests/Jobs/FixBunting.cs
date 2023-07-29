using System;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class FixBunting : Job
    {
        public float Duration = 1.0f;

        public static event Action BuntingFixed;

        public override void Exit()
        {
            BuntingFixed?.Invoke();
        }

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