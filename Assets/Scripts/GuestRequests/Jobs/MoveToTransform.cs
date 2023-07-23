using MyBox;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class MoveToTransform : Job
    {
        public Transform transformTarget;

        public SpriteRenderer followerSprite;
        [SpriteLayer] [SerializeField] private int followerSortingLayer;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private float distanceThreshold = 0.1f;
        private float _enterTargetDistance;

        private Transform _followerTransform;
        private int _prevFollowerSpriteSL;

        public override void Enter()
        {
            base.Enter();

            JobOwner.GetRequestOwner().SetDestination(transformTarget.position);
            _enterTargetDistance =
                Vector3.SqrMagnitude(JobOwner.GetRequestOwner().GetPosition() - transformTarget.position);

            if (followerSprite)
            {
                _followerTransform = followerSprite.GetComponent<Transform>();
                Transform holder = JobOwner.GetRequestOwner().GetHoldingPosition();
                _followerTransform.position = holder.position;

                _prevFollowerSpriteSL = followerSprite.sortingLayerID;
                followerSprite.sortingLayerID = followerSortingLayer;
            }
        }

        public override void Tick(float deltaTime)
        {
            if (!followerSprite)
            {
                return;
            }

            Transform holder = JobOwner.GetRequestOwner().GetHoldingPosition();
            _followerTransform.position = holder.position;
        }

        public override void Exit()
        {
            base.Exit();
            if (followerSprite)
            {
                followerSprite.sortingLayerID = _prevFollowerSpriteSL;
            }
        }

        public override float GetProgressPercentage()
        {
            float sqrDistance =
                Vector3.SqrMagnitude(JobOwner.GetRequestOwner().GetPosition() - transformTarget.position);
            return sqrDistance < distanceThreshold * distanceThreshold
                ? 1.0f
                : (_enterTargetDistance - sqrDistance) / _enterTargetDistance;
        }

        public override float GetTotalDuration()
        {
            return 0.0f;
        }
    }
}