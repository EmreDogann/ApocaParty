using System;
using System.Collections.Generic;
using UnityEngine;

namespace TransformProvider
{
    public class DrinksTableProvider : MonoBehaviour, ITransformProvider
    {
        private readonly List<TransformPair> _drinksSpots = new List<TransformPair>();
        private readonly Dictionary<TransformHandle, TransformPair> _assignedDrinksSpots =
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
                    _drinksSpots.Add(transformPair);
                }
            }

            foreach (TransformPair counterSpot in _drinksSpots)
            {
                TransformHandle handle = new TransformHandle(Guid.NewGuid().GetHashCode());
                _assignedDrinksSpots[handle] = counterSpot;
            }
        }

        public TransformHandle TryAcquireTransform()
        {
            foreach (var entry in _assignedDrinksSpots)
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
            return _assignedDrinksSpots[transformHandle];
        }

        public void ReturnTransform(TransformHandle transformHandle)
        {
            _assignedDrinksSpots[transformHandle].Release();
        }
    }
}