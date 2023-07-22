using AYellowpaper;
using GuestRequests.Requests;
using TransformProvider;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class PlaceAtTarget : Job
    {
        public InterfaceReference<ITransformProvider, MonoBehaviour> transformProvider;
        public SpriteRenderer spriteToPlace;

        private TransformPair transformPair;
        private Transform _followerTransform;

        internal override void Initialize(IJobOwner jobOwner)
        {
            base.Initialize(jobOwner);
            if (transformProvider.Value != null)
            {
                jobOwner.RegisterTransformProvider(transformProvider.Value);
            }
        }

        public override void Enter()
        {
            base.Enter();
            transformPair =
                transformProvider.Value.GetTransformPair(JobOwner.TryGetTransformHandle(transformProvider.Value));
            _followerTransform = spriteToPlace.GetComponent<Transform>();
        }

        public override void Tick(float deltaTime)
        {
            if (!spriteToPlace)
            {
                return;
            }

            _followerTransform.position = transformPair.GetChildTransform().position;
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