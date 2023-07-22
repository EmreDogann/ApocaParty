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
        [ReadOnly] private bool _isClaimed;

        public Transform GetTransform()
        {
            return transform;
        }

        public ConsumedData Consume()
        {
            _isConsumed = true;
            _requestImage.enabled = false;
            ResetRequest();
            return consumeReward;
        }

        public void Claim()
        {
            _isClaimed = true;
        }

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        public bool IsAvailable()
        {
            return !IsConsumed() && !_isClaimed;
        }
    }
}