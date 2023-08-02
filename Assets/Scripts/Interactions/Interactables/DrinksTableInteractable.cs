using Consumable;
using GuestRequests;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;

namespace Interactions.Interactables
{
    public class DrinksTableInteractable : InteractableBase
    {
        [Separator("Drinks Table Settings")]
        [SerializeField] private DrinksTable drinksTable;

        [SerializeField] private string refillTooltip;
        [SerializeField] protected Request refillRequest;
        [SerializeField] protected float hoverScaleAmount = 1.1f;

        private bool _isHovering;

        public override void OnStartHover()
        {
            base.OnStartHover();

            transform.localScale *= hoverScaleAmount;
            _isHovering = true;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            transform.localScale /= hoverScaleAmount;
            _isHovering = false;
        }

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;

            if (!isInteractable && _isHovering)
            {
                OnEndHover();
            }
        }

        public bool IsDrinkAvailable()
        {
            return drinksTable.IsDrinkAvailable();
        }

        [CanBeNull]
        public Drink TryGetDrink()
        {
            return drinksTable.TryGetDrink();
        }

        public Request TryRefill()
        {
            if (refillRequest.IsRequestStarted() || refillRequest.GetRequestOwner() != null ||
                !refillRequest.TryStartRequest())
            {
                return null;
            }

            return refillRequest;
        }

        internal Request GetRequest()
        {
            return refillRequest;
        }

        public override string GetTooltipName()
        {
            return drinksTable.IsDrinkAvailable() ? tooltipName : refillTooltip;
        }
    }
}