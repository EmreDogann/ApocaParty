using UnityEngine;

namespace Consumable
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Drink : MonoBehaviour, IConsumable, IConsumableInternal
    {
        [SerializeField] private ConsumedData consumeReward;

        private bool _isConsumed;
        private bool _isClaimed;
        private SpriteRenderer _spriteRenderer;

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
        }
    }
}