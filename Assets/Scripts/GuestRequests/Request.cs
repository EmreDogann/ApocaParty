using System;
using System.Collections.Generic;
using GuestRequests.Requests;
using MyBox;
using Needs;
using TransformProvider;
using UnityEngine;
using Utils;

namespace GuestRequests
{
    [Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Request : MonoBehaviour, IJobOwner
    {
        private List<ITransformProvider> _requiredTransformProviders = new List<ITransformProvider>();
        private Dictionary<ITransformProvider, TransformHandle> _transformPairHandles =
            new Dictionary<ITransformProvider, TransformHandle>();
        public float TotalDuration { get; private set; }

        [SerializeField] private Transform requestStartingPosition;
        [SerializeField] private NeedType fulfillNeed;
        [MetricsRange(-1.0f, 1.0f)] [SerializeField] private NeedMetrics rewardMetrics;
        private NeedMetrics _currentMetrics;

        [SerializeReference] protected List<Job> _jobs = new List<Job>();
        protected float _totalProgressPercentage;
        protected float _currentTime;

        protected int _currentJobIndex;

        private IRequestOwner _owner;
        private SpriteRenderer requestImage;

        private Vector3 startingPosition;

        protected virtual void Awake()
        {
            foreach (Job job in _jobs)
            {
                job.Initialize(this);
            }

            _totalProgressPercentage = 1.0f;
            requestImage = GetComponent<SpriteRenderer>();

            if (requestStartingPosition == null)
            {
                startingPosition = transform.position;
            }
            else
            {
                startingPosition = requestStartingPosition.position;
            }

            ResetRequest();
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
                ReleaseAllTransformHandles();
                _owner = null;
            }
        }

        public bool IsRequestCompleted()
        {
            return _totalProgressPercentage >= 1.0f;
        }

        public bool IsRequestStarted()
        {
            return _currentJobIndex != -1;
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

        public virtual void ResetRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            foreach (Job job in _jobs)
            {
                TotalDuration += job.GetTotalDuration(_owner);
            }

            ReleaseAllTransformHandles();

            _transformPairHandles = new Dictionary<ITransformProvider, TransformHandle>();
            transform.position = startingPosition;
            _currentMetrics = new NeedMetrics();

            _currentTime = 0.0f;
            _totalProgressPercentage = 0.0f;
            _currentJobIndex = -1;
        }

        public virtual void StartRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            NextJob();
        }

        public bool TryAcquireRequestDependencies()
        {
            foreach (ITransformProvider transformProvider in _requiredTransformProviders)
            {
                TransformHandle handle = transformProvider.TryAcquireTransform();
                if (handle == null)
                {
                    foreach (var entry in _transformPairHandles)
                    {
                        entry.Key.ReturnTransform(entry.Value);
                        _transformPairHandles[entry.Key] = null;
                    }

                    return false;
                }

                _transformPairHandles[transformProvider] = handle;
            }

            return true;
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsRequestStarted() && other.CompareTag("Player"))
            {
                if (_jobs[_currentJobIndex].IsFailed(_owner))
                {
                    _jobs[_currentJobIndex].FailJob(_owner);
                    _owner.OwnerRemoved();
                    _owner = null;

                    ResetRequest();
                }
            }
        }

        private void ReleaseAllTransformHandles()
        {
            foreach (var entry in _transformPairHandles)
            {
                entry.Key.ReturnTransform(entry.Value);
            }

            _transformPairHandles.Clear();
        }

        public TransformHandle TryGetTransformHandle(ITransformProvider transformProvider)
        {
            _transformPairHandles.TryGetValue(transformProvider, out TransformHandle handle);
            return handle;
        }

        public void ReturnTransformHandle(ITransformProvider transformProvider)
        {
            transformProvider.ReturnTransform(_transformPairHandles[transformProvider]);
            _transformPairHandles.Remove(transformProvider);
        }

        public void RegisterTransformProvider(ITransformProvider transformProvider)
        {
            if (!_requiredTransformProviders.Contains(transformProvider))
            {
                _requiredTransformProviders.Add(transformProvider);
            }
        }
    }
}