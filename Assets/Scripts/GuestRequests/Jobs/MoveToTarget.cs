using UnityEngine;

namespace GuestRequests.Jobs
{
    public class MoveToTarget : Job
    {
        public Transform target;
        [SerializeField] private float distanceThreshold = 0.1f;
        private float _enterTargetDistance;

        public override void Enter(IRequestOwner owner)
        {
            base.Enter(owner);
            owner.SetDestination(target.position);
            _enterTargetDistance = Vector3.SqrMagnitude(owner.GetPosition() - target.position);
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            float sqrDistance = Vector3.SqrMagnitude(owner.GetPosition() - target.position);
            return sqrDistance < distanceThreshold * distanceThreshold
                ? 1.0f
                : (_enterTargetDistance - Vector3.SqrMagnitude(owner.GetPosition() - target.position)) /
                  _enterTargetDistance;
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return 0.0f;
        }
    }
}