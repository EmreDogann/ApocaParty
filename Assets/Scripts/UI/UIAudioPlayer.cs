﻿using Audio;
using Events;
using MyBox;
using UI.Components.Buttons;
using UnityEngine;

namespace UI
{
    public class UIAudioPlayer : MonoBehaviour
    {
        [Separator("UI Audio Effects")]
        [SerializeField] private AudioSO clickAudio;
        [SerializeField] private AudioSO hoverAudio;
        [SerializeField] private AudioSO backAudio;
        [SerializeField] private AudioSO pauseAudio;

        private BoolEventListener _onGamePausedEvent;

        private void Awake()
        {
            _onGamePausedEvent = GetComponent<BoolEventListener>();
        }

        private void OnEnable()
        {
            MenuButton.OnButtonHover += OnUIHover;
            MenuButton.OnButtonClick += OnUIClick;
            UIInputManager.OnCancelEvent += OnCancel;
            _onGamePausedEvent.Response.AddListener(OnGamePaused);
        }

        private void OnDisable()
        {
            MenuButton.OnButtonHover -= OnUIHover;
            MenuButton.OnButtonClick -= OnUIClick;
            UIInputManager.OnCancelEvent -= OnCancel;
            _onGamePausedEvent.Response.RemoveListener(OnGamePaused);
        }

        private void OnCancel(bool isPaused)
        {
            if (isPaused)
            {
                backAudio.Play2D();
            }
        }

        private void OnGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                // _audioManager.StopAllEvents(masterBus, false);
                pauseAudio.Play2D();
            }
        }

        private void OnUIHover()
        {
            hoverAudio.Play2D();
        }

        private void OnUIClick()
        {
            clickAudio.Play2D();
        }
    }
}