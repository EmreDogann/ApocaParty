using GuestRequests;
using MyBox;
using UnityEngine;

namespace Interactions.Interactables
{
    public class RequestInteractable : InteractableBase, IInteractableRequest
    {
        [SerializeField] protected float hoverScaleAmount = 1.5f;
        [Separator("Request")]
        [SerializeField] protected Request request;

        private bool _isHovering;

        public override void OnStartHover()
        {
            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
            _isHovering = true;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
            _isHovering = false;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;

            if (!isInteractable && _isHovering)
            {
                OnEndHover();
            }
        }

        public Request GetRequest()
        {
            return request;
        }
    }
}