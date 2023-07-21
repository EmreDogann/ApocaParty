using System;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public abstract class Job
    {
        [field: SerializeReference] public string JobName { get; protected set; }
        protected float _currentTime;

        internal virtual void Initialize() {}

        internal virtual void OnDestroy() {}

        public virtual void Enter(IRequestOwner owner)
        {
            Debug.Log($"Entered job: {JobName}");
            _currentTime = 0.0f;
        }

        public virtual void Tick(float deltaTime, IRequestOwner owner)
        {
            _currentTime += deltaTime;
        }

        public virtual void Exit(IRequestOwner owner)
        {
            Debug.Log($"Exited job: {JobName}");
        }

        public abstract float GetProgressPercentage(IRequestOwner owner);
        public abstract float GetTotalDuration(IRequestOwner owner);
    }
}