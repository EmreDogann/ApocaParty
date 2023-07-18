using UnityEngine;

namespace Audio
{
    public class AudioAutoPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSO audioToPlay;
        [SerializeField] private GameObject attachToGameObject;

        private void Start()
        {
            if (attachToGameObject)
            {
                audioToPlay.PlayAttached(attachToGameObject);
            }
            else
            {
                audioToPlay.Play();
            }
        }

        private void OnDestroy()
        {
            audioToPlay.StopAll();
        }
    }
}