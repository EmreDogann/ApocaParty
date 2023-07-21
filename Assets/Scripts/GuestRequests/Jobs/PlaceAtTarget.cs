using AYellowpaper;
using GuestRequests.Requests;
using Needs;
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

        public override void Enter(IRequestOwner owner, ref NeedMetrics metrics)
        {
            base.Enter(owner, ref metrics);
            transformPair =
                transformProvider.Value.GetTransformPair(JobOwner.TryGetTransformHandle(transformProvider.Value));
            _followerTransform = spriteToPlace.GetComponent<Transform>();
        }

        public override void Tick(float deltaTime, IRequestOwner owner, ref NeedMetrics metrics)
        {
            if (!spriteToPlace)
            {
                return;
            }

            _followerTransform.position = transformPair.GetChildTransform().position;
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