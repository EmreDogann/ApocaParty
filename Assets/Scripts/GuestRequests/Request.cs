using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public abstract class Request : MonoBehaviour
    {
        public float TotalDuration { get; }

        protected readonly List<Job> _jobs = new List<Job>();
        protected float _totalProgressPercentage;
        protected float _currentTime;

        protected int _currentJobIndex;

        protected bool isRequestRunning;

        protected Request()
        {
            float totalDuration = 0.0f;
            foreach (Job job in _jobs)
            {
                totalDuration += job.duration;
            }

            TotalDuration = totalDuration;
        }

        private void Update()
        {
            if (isRequestRunning)
            {
                UpdateRequest(Time.deltaTime);
            }
        }

        public void UpdateRequest(float deltaTime)
        {
            _currentTime += deltaTime;
            _jobs[_currentJobIndex].UpdateJob(deltaTime);

            if (_jobs[_currentJobIndex].GetProgressPercentage() >= 1.0f)
            {
                Debug.Log("Job: " + _jobs[_currentJobIndex].name + " Completed!");
                _currentJobIndex++;
                _totalProgressPercentage += 1.0f / _jobs.Count;
            }

            if (IsRequestCompleted())
            {
                Debug.Log("Request Finished!");
                isRequestRunning = false;
            }
        }

        public bool IsRequestCompleted()
        {
            return _totalProgressPercentage >= 1.0f;
        }

        public float GetProgressPercentage()
        {
            return _totalProgressPercentage + _jobs[_currentJobIndex].GetProgressPercentage();
        }

        [ButtonMethod]
        protected virtual void StartRequest()
        {
            isRequestRunning = true;
        }

        [ButtonMethod]
        protected virtual void CurrentJob()
        {
            Debug.Log(_jobs[_currentJobIndex].name);
        }

        [ButtonMethod]
        protected virtual void NextJob()
        {
            if (_currentJobIndex + 1 == _jobs.Count)
            {
                Debug.Log("Request finished!");
            }
            else
            {
                _currentJobIndex++;
                Debug.Log("Moved to next job!");
            }
        }
    }
}