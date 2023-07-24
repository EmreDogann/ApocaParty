using System;
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

        public event Action<FoodRequest> OnConsumed;

        protected override void Awake()
        {
            base.Awake();
            _requestImage.enabled = false;
        }

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

            OnConsumed?.Invoke(this);
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

        public bool IsAvailable()
        {
            return !IsConsumed() && _requestImage.enabled;
        }

        public void SetStartingPosition(Vector3 position)
        {
            startingPosition = position;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            _requestInteractable.SetInteractableActive(isInteractable);
        }
    }
}