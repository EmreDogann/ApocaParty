using System;
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
            if (Time.timeScale > 0.0f)
            {
                OnCancelEvent?.Invoke(true);
            }
            else
            {
                OnCancelEvent?.Invoke(false);
            }
        }
    }
}