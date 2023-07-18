using UnityEngine;

namespace GuestRequests
{
    public interface IRequestOwner
    {
        public void SetDestination(Vector3 target);

        public Vector3 GetPosition();
        // public void SetAnimation();
    }
}