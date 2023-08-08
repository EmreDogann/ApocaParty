using System;
using Audio;
using Consumable;
using GuestRequests.Jobs;
using Interactions.Interactables;
using MyBox;
using UnityEngine;

namespace GuestRequests.Requests
{
    [RequireComponent(typeof(SpillInteractable), typeof(Collider2D))]
    public class FoodRequest : Request, IConsumable
    {
        [Separator("General")]
        [SerializeField] private AudioSO plateAudio;

        [Separator("Consumable Stats")]
        [SerializeField] private ConsumedData consumeReward;

        [Separator("Spills")]
        [SerializeField] private Sprite spillSprite;
        [SerializeField] private AudioSO spillSound;
        private SpillInteractable _spillInteractable;
        private Collider2D _collider2D;

        private Sprite _originalSprite;
        private int _originalSortLayer;
        private int _originalSortOrder;

        [ReadOnly] private bool _isConsumed;

        public event Action<FoodRequest> OnConsumed;
        public static event Action OnFire;

        private bool _requestFailedTriggered;

        private Cook cookJob;
        private bool _isFoodCooked;

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

        private void OnEnable()
        {
            OnFire += OnFireTriggered;
            StoveInteractable.OnFireExtinguished += OnFireExtinguished;

            foreach (Job job in _jobs)
            {
                if (job is Cook cookingJob)
                {
                    cookJob = cookingJob;
                    cookJob.OnFoodCooked += OnFoodCooked;
                    break;
                }
            }
        }

        private void OnDisable()
        {
            OnFire -= OnFireTriggered;
            StoveInteractable.OnFireExtinguished -= OnFireExtinguished;

            cookJob.OnFoodCooked -= OnFoodCooked;
        }

        private void OnFoodCooked()
        {
            _isFoodCooked = true;
        }

        public override void UpdateRequest(float deltaTime)
        {
            base.UpdateRequest(deltaTime);
            if (IsRequestCompleted())
            {
                plateAudio.Play(transform.position);
            }
            
            if (IsRequestFailed())
            {
                OnFire?.Invoke();
            }
        }

        protected override void RequestFinished()
        {
            _collider2D.enabled = true;
            base.RequestFinished();
        }

        public override void ResetRequest()
        {
            base.ResetRequest();
            _isFoodCooked = false;
        }

        private void OnFireTriggered()
        {
            if (!_isFoodCooked)
            {
                if (IsRequestStarted())
                {
                    _requestFailedTriggered = true;
                    _jobs[CurrentJobIndex].FailJob();

                    if (_jobs[CurrentJobIndex] != cookJob)
                    {
                        _requestFailedTriggered = false;
                        SetSorting(_originalSortLayer, _originalSortOrder);

                        transform.position = startingPosition.position;
                        transform.rotation = startingPosition.rotation;
                    }
                }

                Owner?.OwnerRemoved();
                Owner = null;
            }
        }

        private void OnFireExtinguished()
        {
            if (!_isFoodCooked)
            {
                ResetRequest();
            }

            if (_requestFailedTriggered)
            {
                _requestFailedTriggered = false;
                SetSorting(_originalSortLayer, _originalSortOrder);

                transform.position = startingPosition.position;
                transform.rotation = startingPosition.rotation;
            }
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
            RequestInteractable.SetInteractableActive(false);
            _collider2D.enabled = false;
            ResetRequest();

            SetSorting(_originalSortLayer, _originalSortOrder);

            transform.position = startingPosition.position;
            transform.rotation = startingPosition.rotation;

            OnConsumed?.Invoke(this);
            return consumeReward;
        }

        public void Spill()
        {
            RequestImage.sprite = spillSprite;
            spillSound.Play(transform.position);

            SetSorting(_originalSortLayer, _originalSortOrder);
            RequestInteractable.SetInteractableActive(false);
            _spillInteractable.SetInteractableActive(true);
        }

        public bool IsSpilled()
        {
            return _spillInteractable.IsInteractable || _spillInteractable.IsHoverable;
        }

        public void StartCleanup()
        {
            RequestInteractable.SetInteractableActive(false);
        }

        public void Cleanup()
        {
            Consume();
            RequestImage.sprite = _originalSprite;
        }

        public void Claim()
        {
            RequestInteractable.SetInteractableActive(false);
            ReleaseAllTransformHandles();
        }

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        public bool IsAvailable()
        {
            return !IsConsumed() && RequestImage.enabled;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            RequestInteractable.SetInteractableActive(isInteractable);
        }
    }
}