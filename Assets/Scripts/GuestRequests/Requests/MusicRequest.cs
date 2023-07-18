using System;
using Audio;
using UnityEngine;

namespace GuestRequests.Requests
{
    public class MusicRequest : Request
    {
        public static event Action<AudioSO> OnMusicRequested;

        [SerializeField] private AudioSO _requestedMusic;

        private void Awake()
        {
            foreach (Job job in _jobs)
            {
                job.Initialize();
            }

            OnMusicRequested?.Invoke(_requestedMusic);
        }

        private void OnDestroy()
        {
            foreach (Job job in _jobs)
            {
                job.OnDestroy();
            }
        }

        public void RequestMusicToPlay(AudioSO music)
        {
            _requestedMusic = music;
            OnMusicRequested?.Invoke(music);
        }
    }
}