using GuestRequests;
using UnityEngine;

namespace Interactions.Interactables
{
    public class RequestInteractable : InteractableBase, IInteractableRequest
    {
        [SerializeField] private Request request;
        [SerializeField] private float hoverScaleAmount = 1.5f;

        public override void OnStartHover()
        {
            if (request.IsRequestStarted())
            {
                return;
            }

            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
        }

        public override void OnEndHover()
        {
            if (request.IsRequestStarted())
            {
                return;
            }

            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;
        }

        private bool IsRequestActive()
        {
            return request.IsRequestStarted();
        }

        public Request GetRequest()
        {
            return request;
        }
    }
}