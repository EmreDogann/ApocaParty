using Consumable;
using UnityEngine;

namespace Interactions.Interactables
{
    public class FoodPileInteractable : InteractableBase
    {
        [field: SerializeReference] public FoodPile FoodPile { get; private set; }
        [SerializeField] protected float hoverScaleAmount = 1.5f;

        public override void OnStartHover()
        {
            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;
        }
    }
}