using UnityEngine;

namespace Consumable
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Drink : MonoBehaviour, IConsumable, IConsumableInternal
    {
        [SerializeField] private ConsumedData consumeReward;

        private bool _isConsumed;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
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

        public bool IsConsumed()
        {
            return _isConsumed;
        }

        void IConsumableInternal.ResetConsumable()
        {
            _isConsumed = false;
            _spriteRenderer.enabled = true;
        }
    }
}