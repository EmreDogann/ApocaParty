using System;
using MyBox;
using UnityEngine;

namespace Consumable
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Drink : MonoBehaviour, IConsumable, IConsumableInternal
    {
        [SerializeField] private ConsumedData consumeReward;

        [SerializeField] private bool _isConsumed;
        [ReadOnly] private bool _isClaimed;
        private SpriteRenderer _spriteRenderer;

        public event Action OnClaim;

        private void Awake()
        {
            _isClaimed = false;
            _isConsumed = false;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public ConsumedData Consume()
        {
            _isConsumed = true;
            _spriteRenderer.enabled = false;
            return consumeReward;
        }

        public void Claim()
        {
            _isClaimed = true;
            OnClaim?.Invoke();
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
            return !_isClaimed && !_isConsumed && _spriteRenderer.enabled;
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
            _spriteRenderer.enabled = true;
        }
    }
}