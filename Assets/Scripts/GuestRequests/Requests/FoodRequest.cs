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
        private int _originalSortLayer;
        private int _originalSortOrder;

        [ReadOnly] private bool _isConsumed;

        public event Action<FoodRequest> OnConsumed;

        protected override void Awake()
        {
            base.Awake();
            _originalSortLayer = RequestImage.sortingLayerID;
            _originalSortOrder = RequestImage.sortingOrder;

            RequestImage.enabled = false;
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

        public void SetSorting(int layer, int order)
        {
            RequestImage.sortingLayerID = layer;
            RequestImage.sortingOrder = order;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public ConsumedData Consume()
        {
            _isConsumed = true;
            RequestImage.enabled = false;
            RequestInteractable.SetInteractableActive(true);
            _collider2D.enabled = false;
            ResetRequest();

            RequestImage.sortingLayerID = _originalSortLayer;
            RequestImage.sortingOrder = _originalSortOrder;

            OnConsumed?.Invoke(this);
            return consumeReward;
        }

        public void Spill()
        {
            RequestImage.sprite = spillSprite;
            RequestInteractable.SetInteractableActive(false);
            _spillInteractable.SetInteractableActive(true);
        }

        public bool IsSpilled()
        {
            return _spillInteractable.IsInteractable || _spillInteractable.IsHoverable;
        }

        public void Cleanup()
        {
            Consume();
            RequestImage.sprite = _originalSprite;
        }

        public void Claim()
        {
            RequestInteractable.SetInteractableActive(false);
        }

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        public bool IsAvailable()
        {
            return !IsConsumed() && RequestImage.enabled;
        }

        public void SetStartingPosition(Vector3 position)
        {
            StartingPosition = position;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            RequestInteractable.SetInteractableActive(isInteractable);
        }
    }
}