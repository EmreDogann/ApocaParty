using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Components
{
    public enum MixerGroup
    {
        Master,
        SFX,
        Music
    }
    public class AudioSlider : MonoBehaviour
    {
        public MixerGroup mixer;
        public AudioMixer audioMixer;
        private Slider slider;

        private void Start()
        {
            slider = GetComponent<Slider>();

            float dbVolume = -80.0f;
            switch (mixer)
            {
                case MixerGroup.Master:
                    audioMixer.GetFloat("MasterVolume", out dbVolume);
                    break;
                case MixerGroup.SFX:
                    audioMixer.GetFloat("SFXVolume", out dbVolume);
                    break;
                case MixerGroup.Music:
                    audioMixer.GetFloat("MusicVolume", out dbVolume);
                    break;
            }

            slider.SetValueWithoutNotify(Mathf.Pow(10, dbVolume / 20.0f));
        }

        public void SetVolume(float volume)
        {
            float dbVolume = Mathf.Log10(volume) * 20;
            if (volume == 0.0f)
            {
                dbVolume = -80.0f;
            }

            switch (mixer)
            {
                case MixerGroup.Master:
                    audioMixer.SetFloat("MasterVolume", dbVolume);
                    break;
                case MixerGroup.SFX:
                    audioMixer.SetFloat("SFXVolume", dbVolume);
                    break;
                case MixerGroup.Music:
                    audioMixer.SetFloat("MusicVolume", dbVolume);
                    break;
            }
        }
    }
}