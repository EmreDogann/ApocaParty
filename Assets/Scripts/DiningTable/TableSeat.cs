using Consumable;
using UnityEngine;

namespace DiningTable
{
    public class TableSeat : MonoBehaviour
    {
        [SerializeField] private Transform deliverySpot;
        [SerializeField] private Transform plateTransform;
        private bool _isAssigned;

        private IConsumable _consumable;

        private void Awake()
        {
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

        public void SetFood(IConsumable consumable)
        {
            _consumable = consumable;
        }

        public bool HasFood()
        {
            return _consumable != null;
        }

        public IConsumable GetFood()
        {
            IConsumable consumable = _consumable;
            _consumable = null;
            return consumable;
        }

        public Transform GetPlateTransform()
        {
            return plateTransform;
        }

        public Transform GetSeatTransform()
        {
            return transform;
        }

        public bool IsSeatAvailable()
        {
            return !_isAssigned;
        }

        public void AssignSeat()
        {
            _isAssigned = true;
        }

        public void ReleaseSeat()
        {
            _isAssigned = false;
        }

        public Transform GetDestinationTransform()
        {
            return deliverySpot;
        }
    }
}