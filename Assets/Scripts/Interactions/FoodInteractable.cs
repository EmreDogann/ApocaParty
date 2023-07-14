using GuestRequests.Requests;
using Minion;
using UnityEngine;

namespace Interactions
{
    public class FoodInteractable : InteractableBase
    {
        public FoodRequest request;

        public FoodInteractable(float holdDuration, bool holdInteract, float multipleUse, bool isInteractable) : base(
            holdDuration, holdInteract, multipleUse, isInteractable) {}

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
            Minion.Minion minion = other.transform.parent.GetComponent<Minion.Minion>();
            if (minion == null || minion.GetMinionRole() != MinionRole.Chef)
            {
                return;
            }

            transform.localScale *= 3;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Minion.Minion minion = other.transform.parent.GetComponent<Minion.Minion>();
            if (minion == null || minion.GetMinionRole() != MinionRole.Chef)
            {
                return;
            }

            transform.localScale /= 3;
        }
    }
}