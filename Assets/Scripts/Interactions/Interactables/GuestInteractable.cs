using UnityEngine;

namespace Interactions.Interactables
{
    [RequireComponent(typeof(IWaiterTarget))]
    public class GuestInteractable : InteractableBase
    {
        [SerializeField] private float hoverScaleAmount = 1.1f;
        public IWaiterTarget WaiterTarget { get; private set; }

        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }

        private void Awake()
        {
            WaiterTarget = GetComponent<IWaiterTarget>();
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

        public void SetInteractableActive(bool isInteractable)
        {
            IsInteractable = isInteractable;
            IsHoverable = isInteractable;

            if (!isInteractable && IsHovering)
            {
                OnEndHover();
            }
        }
    }
}