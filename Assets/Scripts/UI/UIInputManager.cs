using Dialogue;
using Events.UnityEvents;
using MyBox;
using UI.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace UI
{
    public class UIInputManager : MonoBehaviour
    {
        public UnityEvent OnCancelEvent;
        [OverrideLabel("On Game Pause Event")] [SerializeField] private BoolEventChannelSO onGamePauseSOEvent;
        private InputAction _cancel;
        private InputSystemUIInputModule _uiInputModule;

        private void Awake()
        {
            _uiInputModule = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<InputSystemUIInputModule>();

            if (_uiInputModule != null)
            {
                _cancel = _uiInputModule.cancel.action;
            }
        }

        private void OnEnable()
        {
            _cancel.started += OnCancel;
        }

        private void OnDisable()
        {
            _cancel.started -= OnCancel;
        }

        public void OnCancel(InputAction.CallbackContext ctx)
        {
            View viewActive = UIManager.Instance.GetCurrentView();
            OnCancelEvent?.Invoke();

            if (!viewActive)
            {
                Time.timeScale = 0.0f;
                onGamePauseSOEvent.Raise(true);
                UIManager.Instance.Show<PauseMenuView>();
            }
            else
            {
                bool viewAvailable = UIManager.Instance.IsOnlyView();
                UIManager.Instance.Back();

                if (viewAvailable)
                {
                    if (!DialogueManager.Instance.DialogueIsPlaying)
                    {
                        Time.timeScale = 1.0f;
                    }

                    onGamePauseSOEvent.Raise(false);
                }
            }
        }

        public void GamePause(bool pause)
        {
            if (pause)
            {
                Time.timeScale = 0.0f;
                onGamePauseSOEvent.Raise(true);
            }
            else
            {
                if (!DialogueManager.Instance.DialogueIsPlaying)
                {
                    Time.timeScale = 1.0f;
                }

                onGamePauseSOEvent.Raise(false);
            }
        }
    }
}