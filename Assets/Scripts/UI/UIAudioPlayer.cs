using Audio;
using MyBox;
using UI.Components.Buttons;
using UnityEngine;

namespace UI
{
    public class UIAudioPlayer : MonoBehaviour
    {
        [Separator("UI Audio Effects")]
        [SerializeField] private SoundEffectSO clickAudio;
        [SerializeField] private SoundEffectSO hoverAudio;
        [SerializeField] private SoundEffectSO backAudio;
        [SerializeField] private SoundEffectSO pauseAudio;

        // private BoolEventListener _onGamePausedEvent;

        private void Awake()
        {
            // _onGamePausedEvent = GetComponent<BoolEventListener>();
        }

        private void OnEnable()
        {
            MenuButton.OnButtonHover += OnUIHover;
            MenuButton.OnButtonClick += OnUIClick;
            // UIInputManager.OnCancelEvent += OnCancel;
            // _onGamePausedEvent.Response.AddListener(OnGamePaused);
        }

        private void OnDisable()
        {
            MenuButton.OnButtonHover -= OnUIHover;
            MenuButton.OnButtonClick -= OnUIClick;
            // UIInputManager.OnCancelEvent -= OnCancel;
            // _onGamePausedEvent.Response.RemoveListener(OnGamePaused);
        }

        // private void OnCancel(bool isPaused)
        // {
        //     if (isPaused)
        //     {
        //         AudioManager.Instance.PlayOneShot(backAudio);
        //     }
        // }

        // private void OnGamePaused(bool isPaused)
        // {
        //     if (isPaused)
        //     {
        //         _audioManager.StopAllEvents(masterBus, false);
        //         _audioManager.PlayOneShot(pauseAudio);
        //     }
        // }

        private void OnUIHover()
        {
            AudioManager.Instance.PlayEffectOneShot(hoverAudio);
        }

        private void OnUIClick()
        {
            AudioManager.Instance.PlayEffectOneShot(clickAudio);
        }
    }
}