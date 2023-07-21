using System;
using System.Collections.Generic;
using MyBox;
using Needs;
using UnityEngine;
using Utils;

namespace GuestRequests
{
    [Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Request : MonoBehaviour
    {
        public float TotalDuration { get; private set; }

        [SerializeField] private NeedType fulfillNeed;
        [MetricsRange(-1.0f, 1.0f)] [SerializeField] private NeedMetrics rewardMetrics;
        private NeedMetrics _currentMetrics;

        [SerializeReference] protected List<Job> _jobs = new List<Job>();
        protected float _totalProgressPercentage;
        protected float _currentTime;

        protected int _currentJobIndex;

        private IRequestOwner _owner;
        private SpriteRenderer requestImage;

        protected virtual void Awake()
        {
            foreach (Job job in _jobs)
            {
                job.Initialize();
            }

            _totalProgressPercentage = 1.0f;
            requestImage = GetComponent<SpriteRenderer>();
        }

        protected virtual void OnDestroy()
        {
            foreach (Job job in _jobs)
            {
                job.OnDestroy();
            }
        }

        public virtual void UpdateRequest(float deltaTime)
        {
            _currentTime += deltaTime;
            _jobs[_currentJobIndex].Tick(deltaTime, _owner, ref _currentMetrics);

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

        public virtual NeedMetrics AcceptRequestReward()
        {
            // TODO: Return _currentMetrics instead. Right now, below is used for testing.
            if (IsRequestCompleted())
            {
                requestImage.enabled = false;
                return rewardMetrics;
            }

            return null;
        }

        public NeedType GetFulfillNeed()
        {
            return fulfillNeed;
        }

        public virtual void StartRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            foreach (Job job in _jobs)
            {
                TotalDuration += job.GetTotalDuration(_owner);
            }

            _currentMetrics = new NeedMetrics();

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
                _jobs[_currentJobIndex].Exit(_owner, ref _currentMetrics);
            }
            else
            {
                if (_currentJobIndex >= 0)
                {
                    _jobs[_currentJobIndex].Exit(_owner, ref _currentMetrics);
                }

                _currentJobIndex++;
                _jobs[_currentJobIndex].Enter(_owner, ref _currentMetrics);
            }
        }
    }
}