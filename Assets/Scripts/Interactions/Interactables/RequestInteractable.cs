using GuestRequests;
using UnityEngine;

namespace Interactions.Interactables
{
    public class RequestInteractable : InteractableBase, IInteractableRequest
    {
        [SerializeField] protected Request request;
        [SerializeField] protected float hoverScaleAmount = 1.5f;

        private bool isHovering;

        public override void OnStartHover()
        {
            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
            isHovering = true;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
            isHovering = false;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;

            if (!isInteractable && isHovering)
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