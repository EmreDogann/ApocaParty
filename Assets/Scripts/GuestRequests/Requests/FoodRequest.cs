using System;
using Consumable;
using Interactions.Interactables;
using MyBox;
using UnityEngine;

namespace GuestRequests.Requests
{
    [RequireComponent(typeof(SpillInteractable), typeof(Collider2D))]
    public class FoodRequest : Request, IConsumable
    {
        [Separator("Consumable Stats")]
        [SerializeField] private ConsumedData consumeReward;
        [SerializeField] private Sprite spillSprite;
        private SpillInteractable _spillInteractable;
        private Collider2D _collider2D;

        private Sprite _originalSprite;

        [ReadOnly] private bool _isConsumed;

        public event Action<FoodRequest> OnConsumed;

        protected override void Awake()
        {
            base.Awake();
            _requestImage.enabled = false;
            _collider2D = GetComponent<Collider2D>();
            _collider2D.enabled = false;

            _spillInteractable = GetComponent<SpillInteractable>();
            _spillInteractable.SetInteractableActive(false);
        }

        public override void ActivateRequest()
        {
            base.ActivateRequest();
            _collider2D.enabled = true;
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
            _collider2D.enabled = false;
            ResetRequest();

            OnConsumed?.Invoke(this);
            return consumeReward;
        }

        public void Spill()
        {
            _requestImage.sprite = spillSprite;
            _requestInteractable.SetInteractableActive(false);
            _spillInteractable.SetInteractableActive(true);
        }

        public bool IsSpilled()
        {
            return _spillInteractable.IsInteractable || _spillInteractable.IsHoverable;
        }

        public void Cleanup()
        {
            Consume();
            _requestImage.sprite = _originalSprite;
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