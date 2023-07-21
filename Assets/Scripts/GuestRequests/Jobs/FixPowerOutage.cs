using Audio;
using Needs;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class FixPowerOutage : Job
    {
        public float Duration = 1.0f;
        [SerializeField] private Transform _playbackPosition;
        public AudioSO FixAudio;
        [SerializeField] private ElectricalBox _electricalBox;

        public override void Exit(IRequestOwner owner, ref NeedMetrics metrics)
        {
            if (_playbackPosition)
            {
                FixAudio.Play(_playbackPosition.position);
            }
            else
            {
                FixAudio.Play2D();
            }

            _electricalBox.PowerFixed();
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