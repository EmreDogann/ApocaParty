using System;
using System.Collections.Generic;
using Events.UnityEvents;
using MyBox;
using UI.Views;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        // [SerializeField] private View inGameView;

        [SerializeField] private View startingView;
        [SerializeField] private bool dontRemoveStartingView;

        private readonly Stack<View> _history = new Stack<View>();

        private View _currentView;
        private View[] _views;

        public static UIManager Instance { get; private set; }

        [OverrideLabel("On Game Pause Event")] [SerializeField] private BoolEventChannelSO onGamePauseSOEvent;

        public static Action<View, View> OnViewShow;
        public static Action<View> OnViewOpen;
        public static Action<View> OnViewClose;

        private void Awake()
        {
            Instance = this;
            // HideCursor();

            _views = FindObjectsOfType(typeof(View), true) as View[];

            if (_views == null)
            {
                Debug.LogWarning("Warning - View Manager: No views in the scene have been found!");
                return;
            }

            for (int i = 0; i < _views.Length; i++)
            {
                _views[i].Initialize();
            }

            if (startingView != null)
            {
                Show(startingView);
                // ShowCursor();
            }
            // else
            // {
            //     if (Instance.inGameView != null)
            //     {
            //         Instance.inGameView.Open();
            //     }
            // }
        }

        private void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        private void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public bool IsOnlyView()
        {
            return Instance._history.Count == 0;
        }

        public bool IsStartingView(View view)
        {
            return view == startingView;
        }

        public View GetCurrentView()
        {
            return Instance._currentView;
        }

        public T GetView<T>() where T : View
        {
            for (int i = 0; i < Instance._views.Length; i++)
            {
                if (Instance._views[i] is T tView)
                {
                    return tView;
                }
            }

            return null;
        }


        public void Show<T>(bool remember = true) where T : View
        {
            for (int i = 0; i < Instance._views.Length; i++)
            {
                if (Instance._views[i] is not T)
                {
                    continue;
                }

                if (Instance._currentView != null)
                {
                    if (remember)
                    {
                        Instance._history.Push(Instance._currentView);
                    }

                    Instance._currentView.Close();
                }

                // if (Instance.inGameView != null)
                // {
                //     Instance.inGameView.Close();
                // }
                // Time.timeScale = 0.0f;
                // onGamePauseSOEvent.Raise(true);
                OnViewShow?.Invoke(Instance._currentView, Instance._views[i]);
                Instance._views[i].Open();
                Instance._currentView = Instance._views[i];
            }

            // ShowCursor();
        }

        public void Show(View view, bool remember = true)
        {
            if (Instance._currentView != null)
            {
                if (remember)
                {
                    Instance._history.Push(Instance._currentView);
                }

                Instance._currentView.Close();
            }

            // if (Instance.inGameView != null)
            // {
            //     Instance.inGameView.Close();
            // }
            // Time.timeScale = 0.0f;
            // onGamePauseSOEvent.Raise(true);
            OnViewShow?.Invoke(Instance._currentView, view);
            view.Open();
            Instance._currentView = view;

            // ShowCursor();
        }

        public void Back()
        {
            if (IsStartingView(Instance._currentView) && dontRemoveStartingView)
            {
                return;
            }

            if (IsOnlyView())
            {
                OnViewClose?.Invoke(Instance._currentView);
                Instance._currentView.Close();
                // Instance.inGameView.Open();

                Instance._currentView = null;

                // Time.timeScale = 1.0f;
                // onGamePauseSOEvent.Raise(false);

                // HideCursor();
                return;
            }

            Show(Instance._history.Peek(), false);
            Instance._history.Pop();
        }
    }
}