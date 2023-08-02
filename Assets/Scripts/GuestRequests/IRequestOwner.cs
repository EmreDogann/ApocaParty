using UnityEngine;

namespace GuestRequests
{
    public enum OwnerType
    {
        Player,
        Minion,
        Guest
    }

    public interface IRequestOwner
    {
        public void SetDestination(Vector3 target);
        public void SetDestinationAndDisplayPath(Vector3 target);

        public Vector3 GetPosition();

        public Transform GetHoldingTransform();

        public void OwnerRemoved();

        public OwnerType GetOwnerType();
    }
}