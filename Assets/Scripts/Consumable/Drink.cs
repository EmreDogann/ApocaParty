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
        private Vector3 originalPosition;

        private void Awake()
        {
            _isClaimed = false;
            _isConsumed = false;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            originalPosition = transform.position;
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
        }

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        public bool IsAvailable()
        {
            return !IsConsumed() && !_isClaimed;
        }

        void IConsumableInternal.ResetConsumable()
        {
            _isConsumed = false;
            _isClaimed = false;
            _spriteRenderer.enabled = true;
            transform.position = originalPosition;
        }
    }
}