using System;

namespace Interactions.Interactables
{
    public class MinionInteractable : InteractableBase
    {
        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }

        public Action OnHoverAction;

        public override void OnStartHover()
        {
            base.OnStartHover();
            IsHovering = true;
            OnHoverAction?.Invoke();
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            IsHovering = false;
            OnHoverAction?.Invoke();
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

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     transform.localScale *= 3;
        // }
        //
        // private void OnTriggerExit2D(Collider2D other)
        // {
        //     transform.localScale /= 3;
        // }
    }
}