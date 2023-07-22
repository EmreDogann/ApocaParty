using System;
using DiningTable;
using Player;
using UnityEngine;

namespace Interactions.Interactables
{
    public class PlateInteractable : InteractableBase
    {
        private TableSeat _seatOwner;
        private int _expectedDeliveryID;
        private bool _isExpectingDelivery;

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
        }

        public override void OnEndInteract()
        {
            base.OnEndInteract();
            IsInteracting = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isExpectingDelivery)
            {
                IWaiter waiter = other.GetComponent<IWaiter>();
                if (waiter != null && waiter.GetWaiterID() == _expectedDeliveryID)
                {
                    _isExpectingDelivery = false;
                    _expectedDeliveryID = 0;
                    _seatOwner.FoodArrival(waiter.GetFood());
                }
            }
        }

        public int AnnounceDelivery()
        {
            _isExpectingDelivery = true;
            _expectedDeliveryID = Guid.NewGuid().GetHashCode();
            return _expectedDeliveryID;
        }

        public void AssignOwner(TableSeat tableSeat)
        {
            _seatOwner = tableSeat;
        }
    }
}