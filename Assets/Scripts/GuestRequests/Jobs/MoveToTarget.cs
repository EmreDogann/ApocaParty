using AYellowpaper;
using GuestRequests.Requests;
using MyBox;
using Needs;
using TransformProvider;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class MoveToTarget : Job
    {
        public InterfaceReference<ITransformProvider, MonoBehaviour> transformProvider;

        public SpriteRenderer followerSprite;
        [SpriteLayer] [SerializeField] private int followerSortingLayer;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField] private float distanceThreshold = 0.1f;
        private float _enterTargetDistance;

        private TransformPair transformPair;
        private Transform _followerTransform;
        private int _prevFollowerSpriteSL;

        internal override void Initialize(IJobOwner jobOwner)
        {
            base.Initialize(jobOwner);

            if (transformProvider.Value != null)
            {
                jobOwner.RegisterTransformProvider(transformProvider.Value);
            }
        }

        public override void Enter(IRequestOwner owner, ref NeedMetrics metrics)
        {
            base.Enter(owner, ref metrics);

            transformPair =
                transformProvider.Value.GetTransformPair(JobOwner.TryGetTransformHandle(transformProvider.Value));
            owner.SetDestination(transformPair.GetParentTransform().position);
            _enterTargetDistance =
                Vector3.SqrMagnitude(owner.GetPosition() - transformPair.GetParentTransform().position);

            if (followerSprite)
            {
                _followerTransform = followerSprite.GetComponent<Transform>();
                _prevFollowerSpriteSL = followerSprite.sortingLayerID;
                followerSprite.sortingLayerID = followerSortingLayer;
            }
        }

        public override void Tick(float deltaTime, IRequestOwner owner, ref NeedMetrics metrics)
        {
            if (!followerSprite)
            {
                return;
            }

            Transform holder = owner.GetHoldingPosition();
            _followerTransform.position = holder.position;
        }

        public override void Exit(IRequestOwner owner, ref NeedMetrics metrics)
        {
            base.Exit(owner, ref metrics);
            if (followerSprite)
            {
                followerSprite.sortingLayerID = _prevFollowerSpriteSL;
            }
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            float sqrDistance =
                Vector3.SqrMagnitude(owner.GetPosition() - transformPair.GetParentTransform().position);
            return sqrDistance < distanceThreshold * distanceThreshold
                ? 1.0f
                : (_enterTargetDistance -
                   Vector3.SqrMagnitude(owner.GetPosition() - transformPair.GetParentTransform().position)) /
                  _enterTargetDistance;
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return 0.0f;
        }
    }
}