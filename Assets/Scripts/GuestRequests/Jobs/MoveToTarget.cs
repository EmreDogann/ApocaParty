using MyBox;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class MoveToTarget : Job
    {
        public Transform target;
        public SpriteRenderer followerSprite;
        [SpriteLayer] [SerializeField] private int followerSortingLayer;

        [SerializeField] private readonly float distanceThreshold = 0.1f;
        private float _enterTargetDistance;
        private Transform _followerTransform;
        private int _prevFollowerSpriteSL;

        public override void Enter(IRequestOwner owner)
        {
            base.Enter(owner);

            owner.SetDestination(target.position);
            _enterTargetDistance = Vector3.SqrMagnitude(owner.GetPosition() - target.position);

            if (followerSprite)
            {
                _followerTransform = followerSprite.GetComponent<Transform>();
                _prevFollowerSpriteSL = followerSprite.sortingLayerID;
                followerSprite.sortingLayerID = followerSortingLayer;
            }
        }

        public override void Tick(float deltaTime, IRequestOwner owner)
        {
            if (!followerSprite)
            {
                return;
            }

            Transform holder = owner.GetHoldingPosition();
            _followerTransform.position = holder.position;
        }

        public override void Exit(IRequestOwner owner)
        {
            base.Exit(owner);
            if (followerSprite)
            {
                followerSprite.sortingLayerID = _prevFollowerSpriteSL;
            }
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