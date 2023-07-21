using Audio;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class Cook : Job
    {
        [SerializeField] private AudioSO _cookingAudio;
        [SerializeField] private Transform _playbackPosition;
        public float Duration = 1.0f;

        public override void Enter(IRequestOwner owner)
        {
            base.Enter(owner);
            _cookingAudio.Play(_playbackPosition.position);
        }

        public override void Exit(IRequestOwner owner)
        {
            _cookingAudio.Stop();
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return Duration;
        }
    }
}