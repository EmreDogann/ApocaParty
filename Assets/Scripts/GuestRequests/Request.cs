using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public class Request : MonoBehaviour
    {
        public float TotalDuration { get; private set; }

        [SerializeReference] protected List<Job> _jobs = new List<Job>();
        protected float _totalProgressPercentage;
        protected float _currentTime;

        protected int _currentJobIndex;

        private IRequestOwner _owner;

        protected virtual void Awake()
        {
            foreach (Job job in _jobs)
            {
                job.Initialize();
            }

            _totalProgressPercentage = 1.0f;
        }

        protected virtual void OnDestroy()
        {
            foreach (Job job in _jobs)
            {
                job.OnDestroy();
            }
        }

        public void UpdateRequest(float deltaTime)
        {
            _currentTime += deltaTime;
            _jobs[_currentJobIndex].Tick(deltaTime, _owner);

            if (_jobs[_currentJobIndex].GetProgressPercentage(_owner) >= 1.0f)
            {
                NextJob();
                _totalProgressPercentage += 1.0f / _jobs.Count;
            }

            if (IsRequestCompleted())
            {
                Debug.Log("Request Finished!");
                _owner = null;
            }
        }

        public bool IsRequestCompleted()
        {
            return _totalProgressPercentage >= 1.0f;
        }

        public float GetCurrentJobProgress()
        {
            return _jobs[_currentJobIndex].GetProgressPercentage(_owner);
        }

        public float GetProgress()
        {
            return _totalProgressPercentage + _jobs[_currentJobIndex].GetProgressPercentage(_owner);
        }

        public void AssignOwner(IRequestOwner owner)
        {
            _owner = owner;
        }

        [ButtonMethod]
        public virtual void StartRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            float totalDuration = 0.0f;
            foreach (Job job in _jobs)
            {
                totalDuration += job.GetTotalDuration(_owner);
            }

            TotalDuration = totalDuration;

            _currentTime = 0.0f;
            _totalProgressPercentage = 0.0f;
            _currentJobIndex = -1;
            NextJob();
        }

        [ButtonMethod]
        protected virtual void CurrentJob()
        {
            Debug.Log(_jobs[_currentJobIndex].JobName);
        }

        [ButtonMethod]
        protected virtual void NextJob()
        {
            if (_currentJobIndex + 1 == _jobs.Count)
            {
                _jobs[_currentJobIndex].Exit(_owner);
            }
            else
            {
                if (_currentJobIndex >= 0)
                {
                    _jobs[_currentJobIndex].Exit(_owner);
                }

                _currentJobIndex++;
                _jobs[_currentJobIndex].Enter(_owner);
            }
        }
    }
}