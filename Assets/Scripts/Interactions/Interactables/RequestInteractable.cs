using Consumable;
using GuestRequests;
using UnityEngine;

namespace Interactions.Interactables
{
    public class RequestInteractable : InteractableBase, IInteractableRequest
    {
        [SerializeField] private Request request;
        [SerializeField] private float hoverScaleAmount = 1.5f;

        private IConsumable _consumable;

        private void Awake()
        {
            if (request is IConsumable consumable)
            {
                _consumable = consumable;
            }
        }

        public override void OnStartHover()
        {
            if (request.IsRequestStarted() && !request.IsRequestCompleted())
            {
                return;
            }

            if (_consumable != null && _consumable.IsClaimed())
            {
                return;
            }

            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
        }

        public override void OnEndHover()
        {
            if (request.IsRequestStarted() && !request.IsRequestCompleted())
            {
                return;
            }

            if (_consumable != null && _consumable.IsClaimed())
            {
                return;
            }

            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            Debug.Log(isInteractable);
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