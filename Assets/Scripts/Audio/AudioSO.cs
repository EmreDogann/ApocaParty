using Events.UnityEvents;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio
{
    [CreateAssetMenu(fileName = "New Sound Effect", menuName = "Audio/New Sound Effect")]
    public class AudioSO : ScriptableObject
    {
        private const float SEMITONES_TO_PITCH_CONVERSION_UNIT = 1.05946f;

        [MustBeAssigned] public AudioClip[] clips;

        [Separator("Clip Settings")]
        [MustBeAssigned] public AudioMixerGroup audioMixer;
        [MinMaxRange(0, 1)] public RangedFloat volume = new RangedFloat(0.5f, 0.5f);

        public bool useSemitones;

        [ConditionalField(nameof(useSemitones))]
        [MinMaxRange(-10, 10)] public RangedInt semitones = new RangedInt(0, 0);

        [ConditionalField(nameof(useSemitones), true)]
        [MinMaxRange(0, 3)] public RangedFloat pitch = new RangedFloat(1.0f, 1.0f);

        [Separator("Playback Order")]
        [SerializeField] private SoundClipPlayOrder playOrder;

        [ReadOnly] [SerializeField] private int currentPlayIndex;

        [Separator("Audio Events")]
        [Tooltip("The Audio Event to trigger when trying to play the audio.")]
        [OverrideLabel("Play Trigger Event")] [SerializeField] private AudioEventChannelSO audioEvent;
        private AudioHandle _audioHandle;

        #region PreviewCode

#if UNITY_EDITOR
        private AudioSource previewer;

        private void OnEnable()
        {
            previewer = EditorUtility
                .CreateGameObjectWithHideFlags("AudioPreview", HideFlags.HideAndDontSave,
                    typeof(AudioSource))
                .GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            DestroyImmediate(previewer.gameObject);
        }

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

        public void Play(Vector2 positionWorldSpace = default)
        {
            if (clips.Length == 0)
            {
                Debug.Log($"No sound clips for {name}");
                return;
            }

            AudioEventData audioEventData = new AudioEventData();
            audioEventData.Volume = Random.Range(volume.Min, volume.Max);
            audioEventData.Pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);

            _audioHandle = audioEvent.RaisePlayEvent(this, audioEventData, positionWorldSpace);
        }

        public void PlayAttached(GameObject gameObject)
        {
            if (clips.Length == 0)
            {
                Debug.Log($"No sound clips for {name}");
                return;
            }

            AudioEventData audioEventData = new AudioEventData();
            audioEventData.Volume = Random.Range(volume.Min, volume.Max);
            audioEventData.Pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);

            _audioHandle = audioEvent.RaisePlayAttachedEvent(this, audioEventData, gameObject);
        }

        private enum SoundClipPlayOrder
        {
            Random,
            InOrder,
            Reverse
        }
    }
}