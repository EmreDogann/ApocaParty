using System;
using Audio;

namespace GuestRequests.Requests
{
    public class MusicRequest : Request
    {
        public static event Action<AudioSO> OnMusicRequested;

        private AudioSO _requestedMusic;

        protected override void Awake()
        {
            base.Awake();
            // OnMusicRequested?.Invoke(_requestedMusic);
        }

        public void RequestMusicToPlay(AudioSO music)
        {
            _requestedMusic = music;
            OnMusicRequested?.Invoke(music);
        }
    }
}