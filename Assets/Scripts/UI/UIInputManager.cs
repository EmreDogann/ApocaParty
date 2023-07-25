using System;
using UI.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace UI
{
    public class UIInputManager : MonoBehaviour
    {
        public static Action<bool> OnCancelEvent;
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
            if (viewActive is not DialogueView)
            {
                OnCancelEvent?.Invoke(viewActive);
            }

            if (!viewActive)
            {
                UIManager.Instance.Show<PauseMenuView>();
            }
            else
            {
                if (viewActive is DialogueView)
                {
                    UIManager.Instance.Show<PauseMenuView>();
                    return;
                }

                UIManager.Instance.Back();
            }
        }
    }
}