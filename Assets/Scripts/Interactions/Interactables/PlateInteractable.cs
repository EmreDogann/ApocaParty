using UnityEngine;

namespace Interactions.Interactables
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlateInteractable : InteractableBase
    {
        public IWaiterTarget WaiterTarget { get; private set; }
        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }
        [SerializeField] private float hoverScaleAmount = 1.5f;

        private void Awake()
        {
            WaiterTarget = transform.parent.GetComponent<IWaiterTarget>();
        }

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
    }
}