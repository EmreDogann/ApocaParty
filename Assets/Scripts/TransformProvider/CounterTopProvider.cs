using System;
using System.Collections.Generic;
using UnityEngine;

namespace TransformProvider
{
    public class CounterTopProvider : MonoBehaviour, ITransformProvider
    {
        private readonly List<TransformPair> _counterSpots = new List<TransformPair>();
        private readonly Dictionary<TransformHandle, TransformPair> _assignedCounterSpots =
            new Dictionary<TransformHandle, TransformPair>();

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    break;
                }

                TransformPair transformPair = child.GetComponent<TransformPair>();

                if (transformPair)
                {
                    _counterSpots.Add(transformPair);
                }
            }

            foreach (TransformPair counterSpot in _counterSpots)
            {
                TransformHandle handle = new TransformHandle(Guid.NewGuid().GetHashCode());
                _assignedCounterSpots[handle] = counterSpot;
            }
        }


        public bool IsTransformAvailable()
        {
            foreach (var entry in _assignedCounterSpots)
            {
                if (!entry.Value.IsAssigned())
                {
                    return true;
                }
            }

            return false;
        }

        public TransformHandle TryAcquireTransform()
        {
            foreach (var entry in _assignedCounterSpots)
            {
                if (!entry.Value.IsAssigned())
                {
                    entry.Value.Assign();
                    return entry.Key;
                }
            }

            return null;
        }

        public TransformPair GetTransformPair(TransformHandle transformHandle)
        {
            return _assignedCounterSpots[transformHandle];
        }

        public void ReturnTransform(TransformHandle transformHandle)
        {
            _assignedCounterSpots[transformHandle].Release();
        }
    }
}