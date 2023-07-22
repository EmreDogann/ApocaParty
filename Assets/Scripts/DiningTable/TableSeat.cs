using System;
using Consumable;
using Interactions.Interactables;
using UnityEngine;

namespace DiningTable
{
    public class TableSeat : MonoBehaviour
    {
        [SerializeField] private PlateInteractable plateInteractable;
        private bool _isAssigned;

        public event Action OnFoodArrival;
        private IConsumable _consumable;

        private void Reset()
        {
            if (transform.childCount > 0)
            {
                plateInteractable = transform.GetChild(0).GetComponent<PlateInteractable>();
            }
        }

        private void Awake()
        {
            plateInteractable.AssignOwner(this);
        }

        public void FoodArrival(IConsumable consumable)
        {
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