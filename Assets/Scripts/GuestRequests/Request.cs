using System;
using System.Collections.Generic;
using GuestRequests.Requests;
using Interactions.Interactables;
using TransformProvider;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Request : MonoBehaviour, IJobOwner
    {
        protected List<ITransformProvider> _requiredTransformProviders = new List<ITransformProvider>();
        protected Dictionary<ITransformProvider, TransformHandle> _transformPairHandles =
            new Dictionary<ITransformProvider, TransformHandle>();
        public float TotalDuration { get; private set; }

        [SerializeField] protected bool resetRequestOnCompletion;
        [SerializeField] protected Transform requestResetPosition;

        [SerializeReference] protected List<Job> _jobs = new List<Job>();
        protected float TotalProgressPercentage;
        protected float CurrentTime;

        protected int CurrentJobIndex;

        protected IRequestOwner _owner;
        protected SpriteRenderer _requestImage;

        protected bool _isRequestSetup;
        protected RequestInteractable _requestInteractable;

        public event Action OnRequestCompleted;

        protected virtual void Awake()
        {
            foreach (Job job in _jobs)
            {
                job.Initialize(this);
            }

            TotalProgressPercentage = 1.0f;
            _requestImage = GetComponent<SpriteRenderer>();
            _requestInteractable = GetComponent<RequestInteractable>();

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
            CurrentTime += deltaTime;
            _jobs[CurrentJobIndex].Tick(deltaTime);

            if (IsRequestFailed())
            {
                _requestInteractable?.SetInteractableActive(true);
            }

            if (_jobs[CurrentJobIndex].GetProgressPercentage() >= 1.0f)
            {
                NextJob();
                TotalProgressPercentage += 1.0f / _jobs.Count;
            }

            if (IsRequestCompleted())
            {
                Debug.Log("Request Finished!");
                ReleaseAllTransformHandles();

                _owner = null;
                OnRequestCompleted?.Invoke();

                _requestInteractable?.SetInteractableActive(true);

                if (resetRequestOnCompletion)
                {
                    ResetRequest();
                }
            }
        }

        public bool IsRequestCompleted()
        {
            return TotalProgressPercentage >= 1.0f;
        }

        public bool IsRequestStarted()
        {
            return CurrentJobIndex != -1;
        }

        public float GetCurrentJobProgress()
        {
            return _jobs[CurrentJobIndex].GetProgressPercentage();
        }

        public float GetProgress()
        {
            return TotalProgressPercentage + _jobs[CurrentJobIndex].GetProgressPercentage();
        }

        public void AssignOwner(IRequestOwner owner)
        {
            _owner = owner;
        }

        public virtual Vector3 GetStartingPosition()
        {
            return transform.position;
        }

        protected virtual void ResetRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            foreach (Job job in _jobs)
            {
                TotalDuration += job.GetTotalDuration();
            }

            ReleaseAllTransformHandles();
            _isRequestSetup = false;

            _transformPairHandles = new Dictionary<ITransformProvider, TransformHandle>();
            transform.position = requestResetPosition.position;
            TotalProgressPercentage = 0.0f;
            CurrentJobIndex = -1;
        }

        public virtual void ActivateRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            if (!_isRequestSetup)
            {
                Debug.LogWarning("Request cannot be activated, needs to be setup first.");
                if (!TryStartRequest())
                {
                    Debug.LogWarning("Request could not be setup, aborting request.");
                    _owner.OwnerRemoved();
                    _owner = null;
                    return;
                }
            }

            _requestInteractable?.SetInteractableActive(false);
            CurrentTime = 0.0f;
            TotalProgressPercentage = 0.0f;
            NextJob();
        }

        public bool TryStartRequest()
        {
            foreach (ITransformProvider transformProvider in _requiredTransformProviders)
            {
                if (_transformPairHandles.TryGetValue(transformProvider, out _))
                {
                    continue;
                }

                TransformHandle handle = transformProvider.TryAcquireTransform();
                if (handle == null)
                {
                    foreach (var entry in _transformPairHandles)
                    {
                        entry.Key.ReturnTransform(entry.Value);
                        _transformPairHandles[entry.Key] = null;
                    }

                    _isRequestSetup = false;
                    return false;
                }

                _transformPairHandles[transformProvider] = handle;
            }

            _isRequestSetup = true;
            return true;
        }

        protected virtual void NextJob()
        {
            if (CurrentJobIndex + 1 == _jobs.Count)
            {
                _jobs[CurrentJobIndex].Exit();
            }
            else
            {
                if (CurrentJobIndex >= 0)
                {
                    _jobs[CurrentJobIndex].Exit();
                }

                CurrentJobIndex++;
                _jobs[CurrentJobIndex].Enter();
            }
        }

        public IRequestOwner GetRequestOwner()
        {
            return _owner;
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

        public bool IsRequestFailed()
        {
            return CurrentJobIndex != -1 && _jobs[CurrentJobIndex].IsFailed();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (IsRequestStarted() && other.CompareTag("Player"))
            {
                if (IsRequestFailed())
                {
                    _jobs[CurrentJobIndex].FailJob();
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
    }
}