using Audio;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class CookJob : Job
    {
        [SerializeField] private AudioSO cookingAudio;
        public float duration = 1.0f;

        public override void Enter(IRequestOwner owner)
        {
            base.Enter(owner);
            cookingAudio.Play(transform.position);
        }

        public override void Exit(IRequestOwner owner)
        {
            cookingAudio.Stop();
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            return Mathf.Clamp01(_currentTime / duration);
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return duration;
        }
    }
}