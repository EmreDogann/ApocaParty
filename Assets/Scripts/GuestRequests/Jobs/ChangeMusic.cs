using Audio;
using GuestRequests.Requests;
using Needs;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class ChangeMusic : Job
    {
        private AudioSO _musicAudio;
        [SerializeField] private Transform _playbackPosition;
        public float Duration = 1.0f;

        internal override void Initialize()
        {
            MusicRequest.OnMusicRequested += OnMusicRequested;
        }

        internal override void OnDestroy()
        {
            MusicRequest.OnMusicRequested -= OnMusicRequested;
        }

        public override void Exit(IRequestOwner owner, ref NeedMetrics metrics)
        {
            if (_playbackPosition)
            {
                _musicAudio.Play(_playbackPosition.position);
            }
            else
            {
                _musicAudio.Play2D();
            }
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return Duration;
        }

        private void OnMusicRequested(AudioSO audio)
        {
            _musicAudio = audio;
        }
    }
}