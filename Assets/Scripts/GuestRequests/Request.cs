using System;
using System.Collections.Generic;
using GuestRequests.Requests;
using Interactions.Interactables;
using MyBox;
using TransformProvider;
using UnityEngine;

namespace GuestRequests
{
    [Serializable]
    public class Request : MonoBehaviour, IJobOwner
    {
        protected List<ITransformProvider> _requiredTransformProviders = new List<ITransformProvider>();
        protected Dictionary<ITransformProvider, TransformHandle> _transformPairHandles =
            new Dictionary<ITransformProvider, TransformHandle>();

        [Separator("On Completion")]
        [OverrideLabel("Reset Request")] [SerializeField] protected bool resetRequest_OnCompletion;
        [OverrideLabel("Return Transforms")] [SerializeField] protected bool returnTransforms_OnCompletion;
        [OverrideLabel("Enable Interactable")] [SerializeField] protected bool enableInteractable_OnCompletion;
        [OverrideLabel("Restore Starting Position")] [SerializeField] protected bool restoreStartPos_OnCompletion;
        [SerializeField] protected Transform startingPosition;

        [Separator("Jobs")]
        [SerializeReference] protected List<Job> _jobs = new List<Job>();
        protected float DurationProgressPercentage;
        protected float TotalProgressPercentage;

        protected int CurrentJobIndex;
        protected int JobsWithDurationCount;

        protected IRequestOwner Owner;
        protected SpriteRenderer RequestImage;
        protected Vector3 StartingPosition;

        protected bool IsRequestSetup;
        protected RequestInteractable RequestInteractable;

        public event Action OnRequestCompleted;

        protected virtual void Awake()
        {
            foreach (Job job in _jobs)
            {
                job.Initialize(this);
                if (job.HasDuration())
                {
                    JobsWithDurationCount++;
                }
            }

            // TotalProgressPercentage = 1.0f;
            RequestImage = transform.GetComponentInChildren<SpriteRenderer>();
            RequestInteractable = GetComponent<RequestInteractable>();

            if (startingPosition != null)
            {
                StartingPosition = startingPosition.position;
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
            _jobs[CurrentJobIndex].Tick(deltaTime);

            if (_jobs[CurrentJobIndex].GetProgressPercentage() >= 1.0f)
            {
                if (_jobs[CurrentJobIndex].HasDuration())
                {
                    DurationProgressPercentage += 1.0f / JobsWithDurationCount;
                }

                TotalProgressPercentage += 1.0f / _jobs.Count;

                NextJob();
            }

            if (IsRequestCompleted())
            {
                RequestFinished();
            }
        }

        protected virtual void RequestFinished()
        {
            Debug.Log("Request Finished!");
            if (returnTransforms_OnCompletion)
            {
                ReleaseAllTransformHandles();
            }

            Owner = null;
            OnRequestCompleted?.Invoke();

            if (enableInteractable_OnCompletion)
            {
                if (RequestInteractable != null)
                {
                    RequestInteractable.SetInteractableActive(true);
                }
            }

            if (resetRequest_OnCompletion)
            {
                ResetRequest();
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
            if (_jobs[CurrentJobIndex].HasDuration())
            {
                return DurationProgressPercentage +
                       _jobs[CurrentJobIndex].GetProgressPercentage() / JobsWithDurationCount;
            }

            return DurationProgressPercentage;
        }

        public void AssignOwner(IRequestOwner owner)
        {
            Owner = owner;
        }

        public virtual Vector3 GetStartingPosition()
        {
            return transform.position;
        }

        public virtual void ResetRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            ReleaseAllTransformHandles();
            IsRequestSetup = false;

            _transformPairHandles = new Dictionary<ITransformProvider, TransformHandle>();
            if (restoreStartPos_OnCompletion)
            {
                transform.position = StartingPosition;
            }

            DurationProgressPercentage = 0.0f;
            TotalProgressPercentage = 0.0f;
            CurrentJobIndex = -1;
        }

        public virtual void ActivateRequest()
        {
            if (_jobs.Count <= 0)
            {
                return;
            }

            if (!IsRequestSetup)
            {
                Debug.LogWarning("Request cannot be activated, needs to be setup first.");
                if (!TryStartRequest())
                {
                    Debug.LogWarning("Request could not be setup, aborting request.");
                    Owner.OwnerRemoved();
                    Owner = null;
                    return;
                }
            }

            if (RequestInteractable != null)
            {
                RequestInteractable.SetInteractableActive(false);
            }

            DurationProgressPercentage = 0.0f;
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
                    }

                    _transformPairHandles.Clear();

                    IsRequestSetup = false;
                    return false;
                }

                _transformPairHandles[transformProvider] = handle;
            }

            IsRequestSetup = true;
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
            return Owner;
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