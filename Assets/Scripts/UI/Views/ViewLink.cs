﻿using UnityEngine;

namespace UI.Views
{
    public class ViewLink : View
    {
        [SerializeField] private View[] linkedViews;

        public override void Initialize()
        {
            base.Initialize();

            foreach (View view in linkedViews)
            {
                view.OnViewOpen += ViewOpened;
                view.OnViewClose += ViewClosed;
            }

            // UIManager.OnViewOpen += ViewOpened;
            UIManager.OnViewShow += ViewShown;
        }

        public void OnDestroy()
        {
            foreach (View view in linkedViews)
            {
                view.OnViewOpen -= ViewOpened;
                view.OnViewClose -= ViewClosed;
            }

            // UIManager.OnViewOpen -= ViewOpened;
            UIManager.OnViewShow -= ViewShown;
        }

        public void ViewOpened(View viewToOpen)
        {
            if (!IsActive())
            {
                Open();
            }
        }

        public void ViewClosed(View viewToClose)
        {
            if (UIManager.Instance.IsOnlyView())
            {
                Close();
            }
        }

        public void ViewShown(View viewToClose, View viewToOpen)
        {
            bool keepOpen = true;
            foreach (View view in linkedViews)
            {
                if (view == viewToOpen)
                {
                    keepOpen = true;
                    break;
                }

                if (view != viewToClose && !view.IsActive())
                {
                    keepOpen = false;
                }
            }

            if (!keepOpen)
            {
                Close();
            }
        }
    }
}