using System.Collections.Generic;
using Events;
using Events.UnityEvents;
using MyBox;
using UnityEngine;

namespace Audio
{
    public class AudioEmitter
    {
        public bool CanPause;
        public bool IsPaused;
        public float DefaultSpatialBlend;
        public AudioSource Source;
        public GameObject AttachedGameObject;
    }

    public class AudioManager : MonoBehaviour
    {
        [Separator("Pooling")]
        [SerializeField] private int audioSourcePoolSize;
        [SerializeField] private GameObject audioSourcePrefab;

        [Separator("Event Channels")]
        [SerializeField] private AudioEventChannelSO sfxAudioChannel;
        [SerializeField] private AudioEventChannelSO musicAudioChannel;
        [SerializeField] private BoolEventListener onPauseEvent;

        private List<AudioEmitter> _audioEmitters;
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

            _audioEmitters = new List<AudioEmitter>();
            _audioHandles = new List<AudioHandle>();

            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                AudioEmitter emitter = CreateAudioEmitter();
                _audioEmitters.Add(emitter);
            }
        }

        private void OnEnable()
        {
            onPauseEvent.Response.AddListener(OnPauseEvent);

            sfxAudioChannel.OnAudioPlay += PlaySoundEffect;
            sfxAudioChannel.OnAudioPlay2D += PlaySoundEffect2D;
            sfxAudioChannel.OnAudioPlayAttached += PlaySoundEffectAttached;
            sfxAudioChannel.OnAudioStop += StopSoundEffect;

            // _musicEventChannel.OnAudioCuePlayRequested += PlayMusicTrack;
            // _musicEventChannel.OnAudioCueStopRequested += StopMusic;

            // _masterVolumeEventChannel.OnEventRaised += ChangeMasterVolume;
            // _musicVolumeEventChannel.OnEventRaised += ChangeMusicVolume;
            // _SFXVolumeEventChannel.OnEventRaised += ChangeSFXVolume;
        }

        private void OnDestroy()
        {
            onPauseEvent.Response.RemoveListener(OnPauseEvent);

            sfxAudioChannel.OnAudioPlay -= PlaySoundEffect;
            sfxAudioChannel.OnAudioPlay2D -= PlaySoundEffect2D;
            sfxAudioChannel.OnAudioPlayAttached -= PlaySoundEffectAttached;
            sfxAudioChannel.OnAudioStop -= StopSoundEffect;

            // _SFXEventChannel.OnAudioCueFinishRequested -= FinishAudioCue;
            // _musicEventChannel.OnAudioCuePlayRequested -= PlayMusicTrack;

            // _musicVolumeEventChannel.OnEventRaised -= ChangeMusicVolume;
            // _SFXVolumeEventChannel.OnEventRaised -= ChangeSFXVolume;
            // _masterVolumeEventChannel.OnEventRaised -= ChangeMasterVolume;
        }

        private void Update()
        {
            foreach (AudioEmitter emitter in _audioEmitters)
            {
                if (emitter.AttachedGameObject)
                {
                    emitter.Source.transform.position = emitter.AttachedGameObject.transform.position;
                }
            }
        }

        public AudioHandle PlaySoundEffect2D(AudioSO audio, AudioEventData audioEventData)
        {
            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = null;

            emitter.Source.outputAudioMixerGroup = audio.audioMixer;
            emitter.Source.transform.position = Vector3.zero;
            emitter.Source.clip = audio.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.spatialBlend = 0;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            AudioHandle handle = new AudioHandle(_currentAudioSourceIndex, audio);
            _audioHandles.Add(handle);

            return handle;
        }

        public AudioHandle PlaySoundEffect(AudioSO audio, AudioEventData audioEventData, Vector3 positionInSpace)
        {
            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = null;

            emitter.Source.outputAudioMixerGroup = audio.audioMixer;
            emitter.Source.transform.position = positionInSpace;
            emitter.Source.clip = audio.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.spatialBlend = emitter.DefaultSpatialBlend;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            AudioHandle handle = new AudioHandle(_currentAudioSourceIndex, audio);
            _audioHandles.Add(handle);

            return handle;
        }

        public AudioHandle PlaySoundEffectAttached(AudioSO audio, AudioEventData audioEventData, GameObject gameObject)
        {
            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = gameObject;

            emitter.Source.outputAudioMixerGroup = audio.audioMixer;
            emitter.Source.transform.position = gameObject.transform.position;
            emitter.Source.clip = audio.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            AudioHandle handle = new AudioHandle(_currentAudioSourceIndex, audio);
            _audioHandles.Add(handle);

            return handle;
        }

        public bool StopSoundEffect(AudioHandle handle)
        {
            int handleIndex = _audioHandles.FindIndex(x => x == handle);

            if (handleIndex < 0)
            {
                return false;
            }

            AudioHandle foundHandle = _audioHandles[handleIndex];

            _audioEmitters[foundHandle.ID].Source.Stop();
            _audioEmitters[foundHandle.ID].IsPaused = false;

            _audioHandles.RemoveAt(handleIndex);
            return true;
        }

        private void OnPauseEvent(bool isPaused)
        {
            if (isPaused)
            {
                foreach (AudioEmitter emitter in _audioEmitters)
                {
                    if (emitter.CanPause && emitter.Source.isPlaying)
                    {
                        emitter.Source.Pause();
                        emitter.IsPaused = true;
                    }
                }
            }
            else
            {
                foreach (AudioEmitter emitter in _audioEmitters)
                {
                    if (emitter.CanPause && emitter.IsPaused)
                    {
                        emitter.Source.Play();
                        emitter.IsPaused = false;
                    }
                }
            }
        }

        private AudioEmitter RequestAudioEmitter()
        {
            int emitterIndex = TryGetAvailableEmitter();

            if (emitterIndex < 0)
            {
                AudioEmitter emitter = CreateAudioEmitter();
                _audioEmitters.Add(emitter);

                _currentAudioSourceIndex = _audioEmitters.Count - 1;
                return emitter;
            }

            return _audioEmitters[emitterIndex];
        }

        private int TryGetAvailableEmitter()
        {
            AudioEmitter emitter = _audioEmitters[_currentAudioSourceIndex];
            if (!emitter.IsPaused && !emitter.Source.isPlaying)
            {
                return _currentAudioSourceIndex;
            }

            for (int i = 0; i < _audioEmitters.Count; i++)
            {
                if (!_audioEmitters[i].IsPaused && !_audioEmitters[i].Source.isPlaying)
                {
                    _currentAudioSourceIndex = i;
                    return _currentAudioSourceIndex;
                }
            }

            return -1;
        }

        private AudioEmitter CreateAudioEmitter()
        {
            AudioEmitter emitter = new AudioEmitter();
            emitter.Source = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
            emitter.DefaultSpatialBlend = emitter.Source.spatialBlend;
            emitter.CanPause = false;
            emitter.IsPaused = false;

            return emitter;
        }
    }
}