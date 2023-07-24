using System;
using Consumable;
using Interactions.Interactables;
using UnityEngine;

namespace DiningTable
{
    public class TableSeat : MonoBehaviour, IWaiterTarget
    {
        private PlateInteractable _plateInteractable;
        [SerializeField] private Transform deliverySpot;
        private bool _isAssigned;

        public event Action OnFoodArrival;
        private IConsumable _consumable;
        private int _waiterID;
        private bool _isAssignedWaiter;

        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            _plateInteractable = transform.GetComponentInChildren<PlateInteractable>();

            if (_plateInteractable == null)
            {
                Debug.LogError(
                    $"ERROR: PlateInteractable not found in seat {transform.name}, table {transform.parent.name}");
            }

            if (deliverySpot == null)
            {
                Debug.LogError(
                    $"ERROR: Delivery transform not found in seat {transform.name}, table {transform.parent.name}");
            }
        }

        public bool IsFoodAvailable()
        {
            return _consumable != null;
        }

        public IConsumable GetFood()
        {
            IConsumable consumable = _consumable;
            _consumable = null;
            _plateInteractable.SetInteractableActive(true);
            return consumable;
        }

        public Transform GetSeatTransform()
        {
            return transform;
        }

        public bool IsSeatAvailable()
        {
            return _isAssigned;
        }

        public void AssignSeat()
        {
            _isAssigned = true;
        }

        public void ReleaseSeat()
        {
            _isAssigned = false;
        }

        public void WaiterInteracted(IWaiter waiter)
        {
            waiter.GetConsumable().GetTransform().position = _plateInteractable.transform.position;
            _plateInteractable.SetInteractableActive(false);

            _consumable = waiter.GetConsumable();
            OnFoodArrival?.Invoke();
            
            _waiterID = 0;
            _isAssignedWaiter = false;
        }

        public Transform GetDestinationTransform()
        {
            return deliverySpot;
        }

        public bool IsAssignedWaiter()
        {
            return _isAssignedWaiter;
        }

        public void GiveWaiterID(int waiterID)
        {
            _waiterID = waiterID;
            _isAssignedWaiter = true;
        }

        public int GetWaiterID()
        {
            return _waiterID;
        }
    }
}