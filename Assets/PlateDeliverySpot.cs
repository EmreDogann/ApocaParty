using System;
using Consumable;
using Player;
using UnityEngine;

public class PlateDeliverySpot : MonoBehaviour
{
    private bool _isExpectingDelivery;
    private int _expectedDeliveryID;
    public event Action<IConsumable> OnDeliveryArrived;

    public void StartDelivery(int id)
    {
        _isExpectingDelivery = true;
        _expectedDeliveryID = id;
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

                OnDeliveryArrived?.Invoke(waiter.GetFood());
            }
        }
    }
}