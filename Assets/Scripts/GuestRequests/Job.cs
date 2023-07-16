using System;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public abstract class Job : MonoBehaviour
    {
        [field: SerializeReference] public string JobName { get; private set; }
        public float duration = 1.0f;
        protected float _currentTime;

        protected Job(string name)
        {
            JobName = name;
        }

        public void UpdateJob(float deltaTime)
        {
            _currentTime += deltaTime;
        }

        public float GetProgressPercentage()
        {
            return Mathf.Clamp01(_currentTime / duration);
        }
    }
}