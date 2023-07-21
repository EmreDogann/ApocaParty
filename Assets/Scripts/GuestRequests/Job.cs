using System;
using GuestRequests.Requests;
using Needs;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public abstract class Job
    {
        [field: SerializeReference] public string JobName { get; protected set; }
        protected float _currentTime;
        protected IJobOwner JobOwner;

        internal virtual void Initialize(IJobOwner jobOwner)
        {
            JobOwner = jobOwner;
        }

        internal virtual void OnDestroy() {}

        public virtual void Enter(IRequestOwner owner, ref NeedMetrics metrics)
        {
            Debug.Log($"Entered job: {JobName}");
            _currentTime = 0.0f;
        }

        public virtual void Tick(float deltaTime, IRequestOwner owner, ref NeedMetrics metrics)
        {
            _currentTime += deltaTime;
        }

        public virtual void Exit(IRequestOwner owner, ref NeedMetrics metrics)
        {
            Debug.Log($"Exited job: {JobName}");
        }

        public virtual void FailJob(IRequestOwner owner) {}

        public abstract float GetProgressPercentage(IRequestOwner owner);
        public abstract float GetTotalDuration(IRequestOwner owner);

        public virtual bool IsFailed(IRequestOwner owner)
        {
            return false;
        }

        public virtual bool IsPaused(IRequestOwner owner)
        {
            return false;
        }
    }
}