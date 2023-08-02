using System;
using GuestRequests.Requests;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public abstract class Job
    {
        [field: SerializeReference] public string JobName { get; protected set; }
        [SerializeField] private bool hasDuration;

        protected float _currentTime;
        protected IJobOwner JobOwner;

        internal virtual void Initialize(IJobOwner jobOwner)
        {
            JobOwner = jobOwner;
        }

        internal virtual void OnDestroy() {}

        public virtual void Enter()
        {
            Debug.Log($"Entered job: {JobName}");
            _currentTime = 0.0f;
        }

        public virtual void Tick(float deltaTime)
        {
            _currentTime += deltaTime;
        }

        public virtual void Exit()
        {
            Debug.Log($"Exited job: {JobName}");
        }

        public virtual void FailJob() {}

        public abstract float GetProgressPercentage();
        public abstract float GetTotalDuration();

        public bool HasDuration()
        {
            return hasDuration;
        }

        public virtual bool IsFailed()
        {
            return false;
        }
    }
}