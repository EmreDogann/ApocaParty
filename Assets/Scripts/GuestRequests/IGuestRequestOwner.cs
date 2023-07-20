using UnityEngine;

namespace GuestRequests
{
    public interface IGuestRequestOwner : IRequestOwner
    {
        public Transform GetSeatTransform();
    }
}