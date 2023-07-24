using Consumable;
using UnityEngine;

namespace Interactions.Interactables
{
    public class SpillInteractable : InteractableBase
    {
        public IConsumable Consumable;
        [SerializeField] protected float hoverScaleAmount = 1.5f;

        private bool isHovering;

        private void Awake()
        {
            Consumable = GetComponent<IConsumable>();
        }

        public override void OnStartHover()
        {
            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
            isHovering = true;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
            isHovering = false;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;

            if (!isInteractable && isHovering)
            {
                OnEndHover();
            }
        }
    }
}