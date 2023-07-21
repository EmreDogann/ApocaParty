using GuestRequests;
using UnityEngine;

namespace Interactions.Interactables
{
    public class RequestInteractable : InteractableBase, IInteractableRequest
    {
        [SerializeField] private Request request;

        public override void OnStartHover()
        {
            if (!request.IsRequestCompleted())
            {
                return;
            }

            base.OnStartHover();

            transform.localScale *= 1.5f;
        }

        public override void OnEndHover()
        {
            if (!request.IsRequestCompleted())
            {
                return;
            }

            base.OnEndHover();
            transform.localScale /= 1.5f;
        }

        private bool IsRequestActive()
        {
            return request.IsRequestCompleted();
        }

        public Request GetRequest()
        {
            return request;
        }
    }
}