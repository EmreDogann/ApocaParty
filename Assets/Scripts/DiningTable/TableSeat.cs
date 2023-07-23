using System;
using Consumable;
using Interactions.Interactables;
using UnityEngine;

namespace DiningTable
{
    public class TableSeat : MonoBehaviour
    {
        private PlateInteractable _plateInteractable;
        private PlateDeliverySpot _deliverySpot;
        private bool _isAssigned;

        public event Action OnFoodArrival;
        private IConsumable _consumable;

        private void Awake()
        {
            Reset();
        }

        private void Reset()
        {
            _plateInteractable = transform.GetComponentInChildren<PlateInteractable>();
            _deliverySpot = transform.GetComponentInChildren<PlateDeliverySpot>();

            if (_plateInteractable == null)
            {
                Debug.LogError(
                    $"ERROR: PlateInteractable not found in seat {transform.name}, table {transform.parent.name}");
            }

            if (_deliverySpot == null)
            {
                Debug.LogError(
                    $"ERROR: PlateDeliverySpot not found in seat {transform.name}, table {transform.parent.name}");
            }
        }

        private void OnEnable()
        {
            _plateInteractable.OnDeliveryStarted += OnDeliveryStarted;
            _deliverySpot.OnDeliveryArrived += OnDeliveryArrived;
        }

        private void OnDisable()
        {
            _plateInteractable.OnDeliveryStarted -= OnDeliveryStarted;
            _deliverySpot.OnDeliveryArrived -= OnDeliveryArrived;
        }

        private void OnDeliveryStarted(int id)
        {
            _deliverySpot.StartDelivery(id);
        }

        private void OnDeliveryArrived(IConsumable consumable)
        {
            consumable.GetTransform().position = _plateInteractable.transform.position;

            _consumable = consumable;
            OnFoodArrival?.Invoke();
        }

        public bool IsFoodAvailable()
        {
            return _consumable != null;
        }

        public IConsumable GetFood()
        {
            _consumable = null;
            return _consumable;
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
    }
}