using UnityEngine;

namespace GuestRequests.Jobs
{
    public class PlaceAtTransform : Job
    {
        public Transform transformTarget;
        public SpriteRenderer spriteToPlace;

        private Transform _followerTransform;

        public override void Enter()
        {
            base.Enter();
            _followerTransform = spriteToPlace.GetComponent<Transform>();
        }

        public override void Tick(float deltaTime)
        {
            if (!spriteToPlace)
            {
                return;
            }

            _followerTransform.position = transformTarget.position;
        }

        public override float GetProgressPercentage()
        {
            return 1.0f;
        }

        public override float GetTotalDuration()
        {
            return 0.0f;
        }
    }
}