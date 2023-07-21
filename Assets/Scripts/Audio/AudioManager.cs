using System.Collections.Generic;
using Events;
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
        private AudioEmitter _musicEmitter;

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
            _musicEmitter = null;

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

            musicAudioChannel.OnAudioPlay += PlayMusic;
            musicAudioChannel.OnAudioPlay2D += PlayMusic2D;
            musicAudioChannel.OnAudioStop += StopMusic;
        }

        private void OnDestroy()
        {
            onPauseEvent.Response.RemoveListener(OnPauseEvent);

            sfxAudioChannel.OnAudioPlay -= PlaySoundEffect;
            sfxAudioChannel.OnAudioPlay2D -= PlaySoundEffect2D;
            sfxAudioChannel.OnAudioPlayAttached -= PlaySoundEffectAttached;
            sfxAudioChannel.OnAudioStop -= StopSoundEffect;

            musicAudioChannel.OnAudioPlay -= PlayMusic;
            musicAudioChannel.OnAudioPlay2D -= PlayMusic2D;
            musicAudioChannel.OnAudioStop -= StopMusic;
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

        public AudioHandle PlaySoundEffect2D(AudioSO audioObj, AudioEventData audioEventData)
        {
            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = null;

            emitter.Source.outputAudioMixerGroup = audioObj.GetAudioMixer();
            emitter.Source.transform.position = Vector3.zero;
            emitter.Source.clip = audioObj.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.spatialBlend = 0;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            AudioHandle handle = new AudioHandle(_currentAudioSourceIndex, audioObj);
            _audioHandles.Add(handle);

            return handle;
        }

        public AudioHandle PlaySoundEffect(AudioSO audioObj, AudioEventData audioEventData, Vector3 positionInSpace)
        {
            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = null;

            emitter.Source.outputAudioMixerGroup = audioObj.GetAudioMixer();
            emitter.Source.transform.position = positionInSpace;
            emitter.Source.clip = audioObj.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.spatialBlend = emitter.DefaultSpatialBlend;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            AudioHandle handle = new AudioHandle(_currentAudioSourceIndex, audioObj);
            _audioHandles.Add(handle);

            return handle;
        }

        public AudioHandle PlaySoundEffectAttached(AudioSO audioObj, AudioEventData audioEventData, GameObject gameObj)
        {
            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = gameObj;

            emitter.Source.outputAudioMixerGroup = audioObj.GetAudioMixer();
            emitter.Source.transform.position = gameObj.transform.position;
            emitter.Source.clip = audioObj.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            AudioHandle handle = new AudioHandle(_currentAudioSourceIndex, audioObj);
            _audioHandles.Add(handle);

            return handle;
        }

        public AudioHandle PlayMusic2D(AudioSO audioObj, AudioEventData audioEventData)
        {
            if (_musicEmitter != null && _musicEmitter.Source.isPlaying)
            {
                // Maybe can do fancy things here like fade out audio instead of a hard stop.
                _musicEmitter.Source.Stop();
            }

            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = null;

            emitter.Source.outputAudioMixerGroup = audioObj.GetAudioMixer();
            emitter.Source.transform.position = Vector3.zero;
            emitter.Source.clip = audioObj.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.spatialBlend = 0;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            _musicEmitter = emitter;
            return AudioHandle.Invalid;
        }

        public AudioHandle PlayMusic(AudioSO audioObj, AudioEventData audioEventData, Vector3 positionInSpace)
        {
            if (_musicEmitter != null && _musicEmitter.Source.isPlaying)
            {
                // Maybe can do fancy things here like fade out audio instead of a hard stop.
                _musicEmitter.Source.Stop();
            }

            AudioEmitter emitter = RequestAudioEmitter();
            emitter.CanPause = audioEventData.CanPause;
            emitter.AttachedGameObject = null;

            emitter.Source.outputAudioMixerGroup = audioObj.GetAudioMixer();
            emitter.Source.transform.position = positionInSpace;
            emitter.Source.clip = audioObj.GetAudioClip();
            emitter.Source.volume = audioEventData.Volume;
            emitter.Source.pitch = audioEventData.Pitch;
            emitter.Source.spatialBlend = emitter.DefaultSpatialBlend;
            emitter.Source.loop = audioEventData.ShouldLoop;
            //Reset in case this AudioSource is being reused for a short SFX after being used for a long music track
            emitter.Source.time = 0f;
            emitter.Source.Play();

            _musicEmitter = emitter;
            return AudioHandle.Invalid;
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

        public bool StopMusic(AudioHandle handle)
        {
            if (_musicEmitter != null && _musicEmitter.Source.isPlaying)
            {
                // Maybe can do fancy things here like fade out audio instead of a hard stop.
                _musicEmitter.Source.Stop();
                return true;
            }

            return false;
        }

        private void OnPauseEvent(bool isPaused)
        {
            foreach (AudioEmitter emitter in _audioEmitters)
            {
                if (isPaused)
                {
                    if (emitter.CanPause && emitter.Source.isPlaying)
                    {
                        emitter.Source.Pause();
                        emitter.IsPaused = true;
                    }
                }
                else
                {
                    if (emitter.IsPaused)
                    {
                        emitter.Source.Play();
                        emitter.IsPaused = false;
                    }
                }
            }

            if (_musicEmitter != null && _musicEmitter.CanPause && _musicEmitter.Source.isPlaying)
            {
                if (isPaused)
                {
                    _musicEmitter.Source.Pause();
                }
                else
                {
                    _musicEmitter.Source.Play();
                }

                _musicEmitter.IsPaused = isPaused;
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