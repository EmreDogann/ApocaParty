using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Views
{
    public class ViewState
    {
        public bool IsOpen;

        public ViewState(bool isOpen)
        {
            IsOpen = isOpen;
        }
    }
    public class ViewGroup : MonoBehaviour, IViewable
    {
        // Tuple item1 -> initialState,
        // Tuple item2 -> currentState.
        private readonly Dictionary<IViewable, Tuple<ViewState, ViewState>> _childViews =
            new Dictionary<IViewable, Tuple<ViewState, ViewState>>();
        private readonly Stack<IViewable> _history = new Stack<IViewable>();

        protected bool _isActive;

        public virtual void Initialize()
        {
            foreach (Transform child in transform)
            {
                IViewable view = child.GetComponent<IViewable>();
                if (view == null)
                {
                    continue;
                }

                ViewState viewState = new ViewState(view.IsActive());
                _childViews.Add(view, new Tuple<ViewState, ViewState>(viewState, viewState));
            }
        }

        public void Open() {}

        public void Close() {}

        public bool IsActive()
        {
            return _isActive;
        }

        public void ResetToInitialState()
        {
            foreach (var entry in _childViews)
            {
                if (entry.Value.Item1.IsOpen)
                {
                    entry.Key.Open();
                }
                else
                {
                    entry.Key.Close();
                }
            }
        }

        public void OpenAll()
        {
            foreach (var entry in _childViews)
            {
                entry.Value.Item2.IsOpen = true;
            }
        }

        public void CloseAll()
        {
            foreach (var entry in _childViews)
            {
                entry.Key.Close();
                entry.Value.Item2.IsOpen = false;
            }
        }
    }
}