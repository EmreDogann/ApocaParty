using GuestRequests;
using Minion;
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

        public override void OnStartInteract()
        {
            base.OnStartInteract();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            MinionAI minion = other.transform.parent.GetComponent<MinionAI>();
            if (minion == null)
            {
                return;
            }

            transform.localScale *= 3;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            MinionAI minion = other.transform.parent.GetComponent<MinionAI>();
            if (minion == null)
            {
                return;
            }

            transform.localScale /= 3;
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