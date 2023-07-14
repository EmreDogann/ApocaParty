using UnityEngine;

namespace GuestRequests
{
    public class Job
    {
        // private readonly List<Action> _actions = new List<Action>();
        // private Minion
        public string name { get; private set; }
        public float duration = 1.0f;
        protected float _currentTime;

        public Job(string name)
        {
            this.name = name;
            // Debug.Log("New job: " + name);
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