using UnityEngine;
using UnityEngine.Audio;

namespace UI
{
    public enum MixerVolume
    {
        Master,
        SFX,
        Music
    }
    public class AudioSlider : MonoBehaviour
    {
        public MixerVolume mixer;
        public AudioMixer audioMixer;

        public void SetVolume(float volume)
        {
            switch (mixer)
            {
                case MixerVolume.Master:
                    audioMixer.SetFloat("MasterVolume", volume);
                    break;
                case MixerVolume.SFX:
                    audioMixer.SetFloat("SFXVolume", volume);
                    break;
                case MixerVolume.Music:
                    audioMixer.SetFloat("MusicVolume", volume);
                    break;
            }
        }
    }
}
