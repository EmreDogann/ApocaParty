using System;
using System.Collections;
using UI.Views.Transitions;
using UnityEngine;

namespace UI.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public class View : MonoBehaviour
    {
        [SerializeField] private MenuTransitionFactory menuTransitionFactory;
        protected Coroutine _coroutine;

        protected bool _isActive;
        private MenuTransition _menuTransition;
        public CanvasGroup CanvasGroup { get; private set; }

        public Action<View> OnViewOpen;
        public Action<View> OnViewClose;

        public virtual void Initialize()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
            CanvasGroup.blocksRaycasts = false;

            _menuTransition = menuTransitionFactory.CreateTransition();
            _menuTransition.Initialize(this);
        }

        public virtual void Open()
        {
            _isActive = true;
            gameObject.SetActive(_isActive);
            OnViewOpen?.Invoke(this);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(Show());
        }

        public virtual void Close()
        {
            if (!_isActive)
            {
                return;
            }

            OnViewClose?.Invoke(this);

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(Hide());
        }

        public bool IsActive()
        {
            return _isActive;
        }

        protected virtual IEnumerator Show()
        {
            yield return _menuTransition.Show(this);
            CanvasGroup.blocksRaycasts = true;
        }

        protected virtual IEnumerator Hide()
        {
            CanvasGroup.blocksRaycasts = false;
            yield return _menuTransition.Hide(this);
            _isActive = false;
            gameObject.SetActive(_isActive);
        }
    }
}