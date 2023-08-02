using System;
using Audio;
using Interactions.Interactables;
using MyBox;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Consumable
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(SpillInteractable))]
    public class Drink : MonoBehaviour, IConsumable, IConsumableInternal
    {
        [SerializeField] private ConsumedData consumeReward;
        [SerializeField] private Sprite spillSprite;
        [SerializeField] private AudioSO spillSound;
        [SpriteLayer] [SerializeField] private int spillSortingLayer;
        [SerializeField] private int spillSortingOrder;

        [ReadOnly] private bool _isConsumed;
        [ReadOnly] private bool _isClaimed;

        private SpillInteractable _spillInteractable;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;
        private Sprite _originalSprite;

        private int _startingSortingLayer;
        private int _startingSortingOrder;
        private Vector3 _startingPosition;

        public event Action<Drink> OnClaim;

        private void Awake()
        {
            _isClaimed = false;
            _isConsumed = false;
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _collider2D = GetComponent<Collider2D>();
            _collider2D.enabled = false;

            _spillInteractable = GetComponent<SpillInteractable>();
            _spillInteractable.SetInteractableActive(false);

            _originalSprite = _spriteRenderer.sprite;
            _spriteRenderer.enabled = false;

            _startingPosition = transform.position;
            _startingSortingLayer = _spriteRenderer.sortingLayerID;
            _startingSortingOrder = _spriteRenderer.sortingOrder;
        }

        public void SetSorting(int layer, int order)
        {
            _spriteRenderer.sortingLayerID = layer;
            _spriteRenderer.sortingOrder = order;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public ConsumedData Consume()
        {
            if (!_isClaimed)
            {
                Claim();
            }

            _isConsumed = true;
            Hide();
            return consumeReward;
        }

        [ButtonMethod]
        public void Spill()
        {
            _spriteRenderer.sprite = spillSprite;
            _collider2D.enabled = true;
            _spillInteractable.SetInteractableActive(true);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(50.0f, 150.0f));

            SetSorting(spillSortingLayer, spillSortingOrder);
            spillSound.Play(transform.position);
        }

        public bool IsSpilled()
        {
            return _spillInteractable.IsInteractable || _spillInteractable.IsHoverable;
        }

        public void Cleanup()
        {
            Consume();
            _spriteRenderer.sprite = _originalSprite;
            _spillInteractable.SetInteractableActive(false);
            transform.localRotation = quaternion.identity;

            SetSorting(_startingSortingLayer, _startingSortingOrder);
        }

        public void Claim()
        {
            _isClaimed = true;
            Show();
            OnClaim?.Invoke(this);
        }

        public bool IsClaimed()
        {
            return _isClaimed;
        }

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        public bool IsAvailable()
        {
            return !_isClaimed && !_isConsumed;
        }

        public void Show()
        {
            _spriteRenderer.enabled = true;
        }

        public void Hide()
        {
            _spriteRenderer.enabled = false;
        }

        public bool IsVisible()
        {
            return _spriteRenderer.enabled;
        }

        void IConsumableInternal.ResetConsumable()
        {
            _isConsumed = false;
            _isClaimed = false;
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;

            transform.position = _startingPosition;
        }
    }
}