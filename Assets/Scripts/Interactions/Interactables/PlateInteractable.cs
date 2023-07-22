using System;
using DiningTable;
using GuestRequests.Requests;

namespace Interactions.Interactables
{
    public class PlateInteractable : InteractableBase
    {
        private TableSeat seatOwner;
        private RequestInteractable requestOnPlate;

        public Action OnPlateInteracted;

        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }

        public override void OnStartHover()
        {
            base.OnStartHover();
            IsHovering = true;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            IsHovering = false;
        }

        public override void OnStartInteract()
        {
            base.OnStartInteract();
            IsInteracting = true;
            OnPlateInteracted?.Invoke();
        }

        public override void OnEndInteract()
        {
            base.OnEndInteract();
            IsInteracting = false;
        }

        public void PlaceRequestOnPlate(RequestInteractable requestInteractable)
        {
            switch (requestInteractable.GetRequest())
            {
                case FoodRequest foodRequest:
                    seatOwner.FoodArrival(foodRequest);
                    break;
            }
        }
    }
}