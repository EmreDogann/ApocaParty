using GuestRequests.Requests;
using Minion;
using UnityEngine;

namespace Interactions.Interactables
{
    public class PowerInteractable : InteractableBase
    {
        [SerializeField] private FoodRequest request;

        public override void OnStartHover()
        {
            base.OnStartHover();
            transform.localScale *= 1.5f;
        }

        public override void OnEndHover()
        {
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

        // public Request GetRequest()
        // {
        //     return request;
        // }
    }
}