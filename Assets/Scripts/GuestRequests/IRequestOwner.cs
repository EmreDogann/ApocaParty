using UnityEngine;

namespace GuestRequests
{
    public interface IRequestOwner
    {
        public void SetDestination(Vector3 target);
        public void SetDestinationAndDisplayPath(Vector3 target);

        public Vector3 GetPosition();

        public Transform GetHoldingPosition();

        public void OwnerRemoved();

        // public void SetAnimation();
    }
}