using System;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public abstract class Job : MonoBehaviour
    {
        [field: SerializeReference] public string JobName { get; protected set; }
        protected float _currentTime;

#if UNITY_EDITOR
        private void Reset()
        {
            JobName = GetType().Name;
        }
#endif

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