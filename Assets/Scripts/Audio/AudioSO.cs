using System.Collections.Generic;
using Events;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "New Audio", menuName = "Audio/New Audio")]
    public class AudioSO : ScriptableObject
    {
        private enum SoundClipPlayOrder
        {
            Random,
            InOrder,
            Reverse
        }

        private const float SEMITONES_TO_PITCH_CONVERSION_UNIT = 1.05946f;

        [MustBeAssigned] [SerializeField] private AudioClip[] clips;

        [Separator("Clip Settings")]
        [MustBeAssigned] [SerializeField] private AudioMixerGroup audioMixer;
        [MinMaxRange(0, 1)] public RangedFloat volume = new RangedFloat(0.5f, 0.5f);

        public bool useSemitones;

        [ConditionalField(nameof(useSemitones))]
        [MinMaxRange(-10, 10)] [SerializeField] private RangedInt semitones = new RangedInt(0, 0);

        [ConditionalField(nameof(useSemitones), true)]
        [MinMaxRange(0, 3)] [SerializeField] private RangedFloat pitch = new RangedFloat(1.0f, 1.0f);

        [Tooltip("Should the audio loop until specified to stop?")]
        [SerializeField] private bool Looping;

        [Tooltip("Should the audio allow being paused when the game is paused.")]
        [SerializeField] private bool CanBePaused;

        [Separator("Playback Order")]
        [SerializeField] private SoundClipPlayOrder playOrder;

        [ReadOnly] [SerializeField] private int currentPlayIndex;

        [Separator("Audio Events")]
        [Tooltip("The Audio Event to trigger when trying to play/stop the audio.")]
        [OverrideLabel("Play Trigger Event")] [SerializeField] private AudioEventChannelSO audioEvent;
        private List<AudioHandle> _audioHandle = new List<AudioHandle>();
        private readonly Dictionary<AudioHandle, AudioEventData>
            _audioHandleData = new Dictionary<AudioHandle, AudioEventData>();

        #region PreviewCode

#if UNITY_EDITOR
        private AudioSource previewer;

        private void OnValidate()
        {
            SyncPitchAndSemitones();
        }


        [ButtonMethod]
        private void PlayPreview()
        {
            PlayPreview(previewer);
        }

        [ButtonMethod]
        private void StopPreview()
        {
            previewer.Stop();
        }

        private void PlayPreview(AudioSource audioSource)
        {
            if (clips.Length == 0)
            {
                Debug.LogError($"No sound clips for {name}");
                return;
            }

            if (audioMixer == null)
            {
                Debug.LogError("No mixer provided, aborting...");
                return;
            }

            AudioSource source = audioSource;
            if (source == null)
            {
                Debug.LogError("No audio source provided, aborting...");
                return;
            }

            source.outputAudioMixerGroup = audioMixer;
            source.clip = GetAudioClip();
            source.volume = Random.Range(volume.Min, volume.Max);
            source.pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);

            source.Play();
        }
#endif

        #endregion

        private void OnEnable()
        {
#if UNITY_EDITOR
            previewer = EditorUtility
                .CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave,
                    typeof(AudioSource))
                .GetComponent<AudioSource>();
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            DestroyImmediate(previewer.gameObject);
#endif

            _audioHandle = new List<AudioHandle>();
        }

        private void SyncPitchAndSemitones()
        {
            if (useSemitones)
            {
                pitch.Min = Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, semitones.Min);
                pitch.Max = Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, semitones.Max);
            }
            else
            {
                semitones.Min =
                    Mathf.RoundToInt(Mathf.Log10(pitch.Min) / Mathf.Log10(SEMITONES_TO_PITCH_CONVERSION_UNIT));
                semitones.Max =
                    Mathf.RoundToInt(Mathf.Log10(pitch.Max) / Mathf.Log10(SEMITONES_TO_PITCH_CONVERSION_UNIT));
            }
        }

        public AudioClip GetAudioClip()
        {
            // Get current clip
            AudioClip clip = clips[currentPlayIndex >= clips.Length ? 0 : currentPlayIndex];

            // Find next clip
            switch (playOrder)
            {
                case SoundClipPlayOrder.InOrder:
                    currentPlayIndex = (currentPlayIndex + 1) % clips.Length;
                    break;
                case SoundClipPlayOrder.Random:
                    currentPlayIndex = Random.Range(0, clips.Length);
                    break;
                case SoundClipPlayOrder.Reverse:
                    currentPlayIndex = (currentPlayIndex + clips.Length - 1) % clips.Length;
                    break;
            }

            return clip;
        }

        public AudioMixerGroup GetAudioMixer()
        {
            return audioMixer;
        }

        public void Play(Vector3 positionWorldSpace = default, bool fadeIn = false)
        {
            if (clips.Length == 0)
            {
                Debug.LogWarning($"No sound clips for {name}");
                return;
            }

            AudioEventData audioEventData = new AudioEventData();
            audioEventData.Volume = Random.Range(volume.Min, volume.Max);
            audioEventData.Pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);
            audioEventData.ShouldLoop = Looping;
            audioEventData.CanPause = CanBePaused;
            if (fadeIn)
            {
                audioEventData.SoundFade = new SoundFade
                {
                    FadeType = FadeType.FadeIn,
                    Volume = audioEventData.Volume,
                    Duration = 1.0f
                };
            }

            AudioHandle handle = audioEvent.RaisePlayEvent(this, audioEventData, positionWorldSpace);
            if (handle != AudioHandle.Invalid)
            {
                _audioHandle.Add(handle);
                _audioHandleData.TryAdd(handle, audioEventData);
            }
        }

        public void Play2D(bool fadeIn = false)
        {
            if (clips.Length == 0)
            {
                Debug.LogWarning($"No sound clips for {name}");
                return;
            }

            AudioEventData audioEventData = new AudioEventData();
            audioEventData.Volume = Random.Range(volume.Min, volume.Max);
            audioEventData.Pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);
            audioEventData.ShouldLoop = Looping;
            audioEventData.CanPause = CanBePaused;
            if (fadeIn)
            {
                audioEventData.SoundFade = new SoundFade
                {
                    FadeType = FadeType.FadeIn,
                    Volume = audioEventData.Volume,
                    Duration = 1.0f
                };
            }

            AudioHandle handle = audioEvent.RaisePlay2DEvent(this, audioEventData);
            if (handle != AudioHandle.Invalid)
            {
                _audioHandle.Add(handle);
                _audioHandleData.TryAdd(handle, audioEventData);
            }
        }

        public void PlayAttached(GameObject gameObject, bool fadeIn = false)
        {
            if (clips.Length == 0)
            {
                Debug.LogWarning($"No sound clips for {name}");
                return;
            }

            AudioEventData audioEventData = new AudioEventData();
            audioEventData.Volume = Random.Range(volume.Min, volume.Max);
            audioEventData.Pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);
            audioEventData.ShouldLoop = Looping;
            audioEventData.CanPause = CanBePaused;
            if (fadeIn)
            {
                audioEventData.SoundFade = new SoundFade
                {
                    FadeType = FadeType.FadeIn,
                    Volume = audioEventData.Volume,
                    Duration = 1.0f
                };
            }

            AudioHandle handle = audioEvent.RaisePlayAttachedEvent(this, audioEventData, gameObject);
            if (handle != AudioHandle.Invalid)
            {
                _audioHandle.Add(handle);
                _audioHandleData.TryAdd(handle, audioEventData);
            }
        }

        public void StopAll()
        {
            if (_audioHandle.Count < 1)
            {
                audioEvent.RaiseStopEvent(AudioHandle.Invalid);
                return;
            }

            foreach (AudioHandle handle in _audioHandle)
            {
                bool handleFound = audioEvent.RaiseStopEvent(handle);

                if (!handleFound)
                {
                    Debug.LogWarning($"Audio {handle.Audio.name} could not be stopped. Handle is stale.");
                }
            }

            _audioHandle.Clear();
            _audioHandleData.Clear();
        }

        public void Stop()
        {
            if (_audioHandle.Count < 1)
            {
                audioEvent.RaiseStopEvent(AudioHandle.Invalid);
                return;
            }

            AudioHandle handle = _audioHandle[^1];
            bool handleFound = audioEvent.RaiseStopEvent(handle);

            if (!handleFound)
            {
                Debug.LogWarning($"Audio {handle.Audio.name} could not be stopped. Handle is stale.");
            }

            _audioHandle.RemoveAt(_audioHandle.Count - 1);
            _audioHandleData.Remove(handle);
        }

        public void FadeAudio(float to, float duration)
        {
            if (_audioHandle.Count < 1)
            {
                return;
            }

            AudioHandle handle = _audioHandle[^1];
            audioEvent.OnAudioFade(handle, to, duration);
        }

        public void UnFadeAudio(float duration)
        {
            if (_audioHandle.Count < 1)
            {
                return;
            }

            AudioHandle handle = _audioHandle[^1];
            audioEvent.OnAudioFade(handle, _audioHandleData[handle].Volume, duration);
        }
    }
}