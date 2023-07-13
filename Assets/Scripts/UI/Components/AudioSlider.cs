using UnityEngine;
using UnityEngine.Audio;

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

        public void SetVolume(float volume)
        {
            switch (mixer)
            {
                case MixerGroup.Master:
                    audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
                    break;
                case MixerGroup.SFX:
                    audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
                    break;
                case MixerGroup.Music:
                    audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
                    break;
            }
        }
    }
}