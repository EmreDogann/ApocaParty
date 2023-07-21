using System;
using System.Collections.Generic;
using Electricity;
using UnityEngine;

namespace TransformProvider
{
    public class KitchenTopProvider : MonoBehaviour, ITransformProvider
    {
        private readonly List<Stove> _kitchenAppliances = new List<Stove>();
        private readonly Dictionary<TransformHandle, Stove> _assignedKitchenAppliances =
            new Dictionary<TransformHandle, Stove>();

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    break;
                }

                Stove electricalAppliance = child.GetComponent<Stove>();
                if (electricalAppliance != null)
                {
                    _kitchenAppliances.Add(electricalAppliance);
                }
            }

            foreach (Stove kitchenAppliance in _kitchenAppliances)
            {
                TransformHandle handle = new TransformHandle(Guid.NewGuid().GetHashCode());
                _assignedKitchenAppliances[handle] = kitchenAppliance;
            }
        }

        public TransformHandle TryAcquireTransform()
        {
            foreach (var entry in _assignedKitchenAppliances)
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
            return _assignedKitchenAppliances[transformHandle].GetTransformPair();
        }

        public void ReturnTransform(TransformHandle transformHandle)
        {
            _assignedKitchenAppliances[transformHandle].Release();
        }
    }
}