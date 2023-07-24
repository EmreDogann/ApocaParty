using UnityEngine;

namespace Interactions.Interactables
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlateInteractable : InteractableBase
    {
        public IWaiterTarget WaiterTarget { get; private set; }
        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }
        [SerializeField] private float hoverScaleAmount = 1.1f;
        private Vector3 _startingScale;

        private void Awake()
        {
            WaiterTarget = transform.parent.GetComponent<IWaiterTarget>();
            _startingScale = transform.localScale;
        }

        public override void OnStartHover()
        {
            base.OnStartHover();
            IsHovering = true;

            transform.localScale = _startingScale * hoverScaleAmount;
        }

        public override void OnEndHover()
        {
            base.OnEndHover();
            IsHovering = false;

            transform.localScale = _startingScale;
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