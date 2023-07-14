using MyBox;
using UnityEditor;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu(fileName = "New Sound Effect", menuName = "Audio/New Sound Effect")]
    public class SoundEffectSO : ScriptableObject
    {
        #region config

        private static readonly float SEMITONES_TO_PITCH_CONVERSION_UNIT = 1.05946f;

        [MustBeAssigned] public AudioClip[] clips;

        [MinMaxRange(0, 1)]
        public RangedFloat volume = new RangedFloat(0.5f, 0.5f);

        [Separator]

        //Pitch / Semitones
        public bool useSemitones;

        [ConditionalField(nameof(useSemitones))]
        [MinMaxRange(-10, 10)]
        public RangedInt semitones = new RangedInt(0, 0);

        [ConditionalField(nameof(useSemitones), true)]
        [MinMaxRange(0, 3)]
        public RangedFloat pitch = new RangedFloat(1.0f, 1.0f);

        [Separator]
        [SerializeField] private SoundClipPlayOrder playOrder;

        [ReadOnly] [SerializeField]
        private int playIndex;

        #endregion

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
            Play(previewer);
        }

        [ButtonMethod]
        private void StopPreview()
        {
            previewer.Stop();
        }
#endif

        #endregion

        public void SyncPitchAndSemitones()
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

        private AudioClip GetAudioClip()
        {
            // get current clip
            AudioClip clip = clips[playIndex >= clips.Length ? 0 : playIndex];

            // find next clip
            switch (playOrder)
            {
                case SoundClipPlayOrder.in_order:
                    playIndex = (playIndex + 1) % clips.Length;
                    break;
                case SoundClipPlayOrder.random:
                    playIndex = Random.Range(0, clips.Length);
                    break;
                case SoundClipPlayOrder.reverse:
                    playIndex = (playIndex + clips.Length - 1) % clips.Length;
                    break;
            }

            // return clip
            return clip;
        }

        public AudioSource Play(AudioSource audioSourceParam = null)
        {
            if (clips.Length == 0)
            {
                Debug.Log($"Missing sound clips for {name}");
                return null;
            }

            AudioSource source = audioSourceParam;
            if (source == null)
            {
                GameObject _obj = new GameObject("Sound", typeof(AudioSource));
                source = _obj.GetComponent<AudioSource>();
            }

            // set source config:
            source.clip = GetAudioClip();
            source.volume = Random.Range(volume.Min, volume.Max);
            source.pitch = useSemitones
                ? Mathf.Pow(SEMITONES_TO_PITCH_CONVERSION_UNIT, Random.Range(semitones.Min, semitones.Max))
                : Random.Range(pitch.Min, pitch.Max);

            source.Play();

#if UNITY_EDITOR
            // if (source != previewer)
            // {
            //     Destroy(source.gameObject, source.clip.length / source.pitch);
            // }
#else
                Destroy(source.gameObject, source.clip.length / source.pitch);
#endif
            return source;
        }

        private enum SoundClipPlayOrder
        {
            random,
            in_order,
            reverse
        }
    }
}