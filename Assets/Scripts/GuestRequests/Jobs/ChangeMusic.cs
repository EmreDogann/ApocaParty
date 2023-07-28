using PartyEvents;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class ChangeMusic : Job
    {
        public float Duration = 1.0f;
        public MusicPlayEvent MusicPlayEvent;

        public override void Exit()
        {
            base.Exit();
            MusicPlayEvent.TriggerEvent();
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