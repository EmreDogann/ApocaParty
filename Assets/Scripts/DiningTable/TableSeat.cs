using System;
using GuestRequests.Requests;
using Interactions.Interactables;
using UnityEngine;

namespace DiningTable
{
    public class TableSeat : MonoBehaviour
    {
        [SerializeField] private PlateInteractable plateInteractable;
        private bool _isAssigned;

        public event Action<FoodRequest> OnFoodArrival;
        public event Action OnFoodComing;

        private void Reset()
        {
            if (transform.childCount > 0)
            {
                plateInteractable = transform.GetChild(0).GetComponent<PlateInteractable>();
            }
        }

        private void OnEnable()
        {
            plateInteractable.OnPlateInteracted += FoodEnRoute;
        }

        private void OnDisable()
        {
            plateInteractable.OnPlateInteracted -= FoodEnRoute;
        }

        public void FoodArrival(FoodRequest foodRequest)
        {
            OnFoodArrival?.Invoke(foodRequest);
        }

        public void FoodEnRoute()
        {
            OnFoodComing?.Invoke();
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