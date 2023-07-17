using System.Collections.Generic;
using Events.UnityEvents;
using MyBox;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Separator("Pooling")]
        [SerializeField] private int audioSourcePoolSize;
        [SerializeField] private GameObject audioSourcePrefab;

        [Separator("Event Channels")]
        [SerializeField] private AudioEventChannelSO sfxAudioChannel;
        [SerializeField] private AudioEventChannelSO musicAudioChannel;

        private List<AudioSource> _audioSources;
        private List<AudioHandle> _audioHandles;
        private int _currentAudioSourceIndex;

        private void Awake()
        {
            if (audioSourcePrefab.GetComponent<AudioSource>() == null)
            {
                Debug.LogError(
                    "ERROR (Audio Manager -> Awake()): Provided audio source prefab contains no audio source component! Aborting audio manager initialization...");
                return;
            }

            _audioSources = new List<AudioSource>();
            _audioHandles = new List<AudioHandle>();

            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                GameObject obj = Instantiate(audioSourcePrefab, transform);
                _audioSources.Add(obj.GetComponent<AudioSource>());
            }
        }

        private void OnEnable()
        {
            sfxAudioChannel.OnAudioPlay += PlaySoundEffect;
            // sfxAudioChannel.OnAudioCueStopRequested += StopAudioCue;
            // sfxAudioChannel.OnAudioCueFinishRequested += FinishAudioCue;

            // _musicEventChannel.OnAudioCuePlayRequested += PlayMusicTrack;
            // _musicEventChannel.OnAudioCueStopRequested += StopMusic;

            // _masterVolumeEventChannel.OnEventRaised += ChangeMasterVolume;
            // _musicVolumeEventChannel.OnEventRaised += ChangeMusicVolume;
            // _SFXVolumeEventChannel.OnEventRaised += ChangeSFXVolume;
        }

        private void OnDestroy()
        {
            sfxAudioChannel.OnAudioPlay -= PlaySoundEffect;
            // _SFXEventChannel.OnAudioCuePlayRequested -= PlayAudioCue;
            // _SFXEventChannel.OnAudioCueStopRequested -= StopAudioCue;

            // _SFXEventChannel.OnAudioCueFinishRequested -= FinishAudioCue;
            // _musicEventChannel.OnAudioCuePlayRequested -= PlayMusicTrack;

            // _musicVolumeEventChannel.OnEventRaised -= ChangeMusicVolume;
            // _SFXVolumeEventChannel.OnEventRaised -= ChangeSFXVolume;
            // _masterVolumeEventChannel.OnEventRaised -= ChangeMasterVolume;
        }

        public AudioHandle PlaySoundEffect(AudioSO audio, AudioEventData audioEventData,
            Vector2 positionInSpace)
        {
            AudioSource source = RequestAudioSource();
            source.outputAudioMixerGroup = audio.audioMixer;
            source.clip = audio.GetAudioClip();
            source.volume = audioEventData.Volume;
            source.pitch = audioEventData.Pitch;
            source.loop = audioEventData.ShouldLoop;
            source.time =
                0f; //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            source.Play();

            return new AudioHandle(_currentAudioSourceIndex, audio);
        }

        private AudioSource RequestAudioSource()
        {
            AudioSource source = TryGetAvailableSource();
            if (source == null)
            {
                source = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
                _audioSources.Add(source);

                _currentAudioSourceIndex = _audioSources.Count - 1;
            }

            return source;
        }

        private AudioSource TryGetAvailableSource()
        {
            if (!_audioSources[_currentAudioSourceIndex].isPlaying)
            {
                _currentAudioSourceIndex += 1 % _audioSources.Count;
                return _audioSources[_currentAudioSourceIndex];
            }

            for (int i = 0; i < _audioSources.Count; i++)
            {
                if (!_audioSources[i].isPlaying)
                {
                    _currentAudioSourceIndex = i;
                    return _audioSources[i];
                }
            }

            return null;
        }
    }
}