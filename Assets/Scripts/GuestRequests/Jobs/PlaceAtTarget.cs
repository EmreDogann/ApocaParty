using Needs;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class PlaceAtTarget : Job
    {
        public Transform target;
        public SpriteRenderer spriteToPlace;

        private Transform _followerTransform;

        public override void Enter(IRequestOwner owner, ref NeedMetrics metrics)
        {
            base.Enter(owner, ref metrics);
            _followerTransform = spriteToPlace.GetComponent<Transform>();
        }

        public override void Tick(float deltaTime, IRequestOwner owner, ref NeedMetrics metrics)
        {
            if (!spriteToPlace)
            {
                return;
            }

            _followerTransform.position = target.position;
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            return 1.0f;
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return 0.0f;
        }
    }
}