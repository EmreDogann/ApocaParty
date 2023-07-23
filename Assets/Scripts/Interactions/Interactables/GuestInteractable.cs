using System;
using Player;
using UnityEngine;

namespace Interactions.Interactables
{
    public class GuestInteractable : InteractableBase
    {
        [SerializeField] private float hoverScaleAmount = 1.1f;
        private int _playerID;
        private bool _isPlayerComing;

        public bool IsHovering { get; private set; }
        public bool IsInteracting { get; private set; }

        public Action OnPlayerInteract;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            CheckForPlayer(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            CheckForPlayer(other);
        }

        private void CheckForPlayer(Collider2D other)
        {
            if (_isPlayerComing)
            {
                IWaiter waiter = other.GetComponent<IWaiter>();
                if (waiter != null && waiter.GetWaiterID() == _playerID)
                {
                    _isPlayerComing = false;
                    _playerID = 0;

                    waiter.FinishInteraction();
                    OnPlayerInteract?.Invoke();
                }
            }
        }

        public int PlayerInteracted()
        {
            _isPlayerComing = true;
            _playerID = Guid.NewGuid().GetHashCode();
            return _playerID;
        }
    }
}