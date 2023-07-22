using System;
using UnityEngine;

namespace GuestRequests.Requests
{
    public class DrinkRefillRequest : Request
    {
        public static Action OnDrinkRefill;

        public override void UpdateRequest(float deltaTime)
        {
            base.UpdateRequest(deltaTime);
            if (IsRequestCompleted())
            {
                OnDrinkRefill?.Invoke();
            }
        }

        public override Vector3 GetStartingPosition()
        {
            return requestResetPosition.position;
        }
    }
}