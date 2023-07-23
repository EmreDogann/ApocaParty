using Consumable;
using MyBox;
using UnityEngine;

namespace GuestRequests.Requests
{
    public class FoodRequest : Request, IConsumable
    {
        [Separator("Consumable Stats")]
        [SerializeField] private ConsumedData consumeReward;

        [ReadOnly] private bool _isConsumed;

        public Transform GetTransform()
        {
            return transform;
        }

        public ConsumedData Consume()
        {
            _isConsumed = true;
            _requestImage.enabled = false;
            _requestInteractable.SetInteractableActive(true);

            ResetRequest();
            return consumeReward;
        }

        public void Claim()
        {
            _requestInteractable.SetInteractableActive(false);
        }

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        public bool IsOnTable()
        {
            return !IsConsumed() && _requestImage.enabled;
        }
    }
}