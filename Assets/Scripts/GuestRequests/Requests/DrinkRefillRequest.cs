using UnityEngine;

namespace GuestRequests.Requests
{
    public class DrinkRefillRequest : Request
    {
        public override Vector3 GetStartingPosition()
        {
            return StartingPosition;
        }
    }
}