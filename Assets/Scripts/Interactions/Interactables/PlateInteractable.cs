using System;
using UnityEngine;

namespace Interactions.Interactables
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlateInteractable : InteractableBase
    {
        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }
        [SerializeField] private float hoverScaleAmount = 1.5f;

        public event Action<int> OnDeliveryStarted;

        public override void OnStartHover()
        {
            base.OnStartHover();
            IsHovering = true;

            transform.localScale *= hoverScaleAmount;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            IsHovering = false;

            transform.localScale /= hoverScaleAmount;
        }

        public override void OnStartInteract()
        {
            base.OnStartInteract();
            IsInteracting = true;
        }

        public override void OnEndInteract()
        {
            base.OnEndInteract();
            IsInteracting = false;
        }

        public int AnnounceDelivery()
        {
            int id = Guid.NewGuid().GetHashCode();
            OnDeliveryStarted?.Invoke(id);
            return id;
        }

        public Transform GetDeliveryPosition()
        {
            return transform;
        }
    }
}