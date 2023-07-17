using UnityEngine;

namespace Audio
{
    public class AudioAutoPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSO audioToPlay;

        private void Start()
        {
            audioToPlay.Play();
        }
    }
}