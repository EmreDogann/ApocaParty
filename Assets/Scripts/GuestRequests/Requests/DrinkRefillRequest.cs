using System;

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
    }
}