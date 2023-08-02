using System;
using System.Collections.Generic;
using Electricity;
using UnityEngine;

namespace TransformProvider
{
    public class KitchenTopProvider : MonoBehaviour, ITransformProvider
    {
        private readonly List<TransformPair> _transformPairs = new List<TransformPair>();
        private readonly List<IElectricalAppliance> _kitchenAppliances = new List<IElectricalAppliance>();
        private readonly Dictionary<TransformHandle, Tuple<TransformPair, IElectricalAppliance>>
            _assignedKitchenAppliances =
                new Dictionary<TransformHandle, Tuple<TransformPair, IElectricalAppliance>>();

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
                if (transformPair != null)
                {
                    _transformPairs.Add(transformPair);
                }

                IElectricalAppliance electricalAppliance = child.GetComponent<IElectricalAppliance>();
                if (electricalAppliance != null)
                {
                    _kitchenAppliances.Add(electricalAppliance);
                }

                TransformHandle handle = new TransformHandle(Guid.NewGuid().GetHashCode());
                _assignedKitchenAppliances[handle] =
                    new Tuple<TransformPair, IElectricalAppliance>(transformPair, electricalAppliance);
            }
        }

        public bool IsTransformAvailable()
        {
            foreach (var entry in _assignedKitchenAppliances)
            {
                if (!entry.Value.Item1.IsAssigned())
                {
                    return true;
                }
            }

            return false;
        }

        public TransformHandle TryAcquireTransform()
        {
            foreach (var entry in _assignedKitchenAppliances)
            {
                if (!entry.Value.Item1.IsAssigned())
                {
                    entry.Value.Item1.Assign();
                    return entry.Key;
                }
            }

            return null;
        }

        public void TurnOnAppliance(TransformHandle transformHandle)
        {
            _assignedKitchenAppliances[transformHandle].Item2.TurnOn();
        }

        public void TurnOffAppliance(TransformHandle transformHandle)
        {
            _assignedKitchenAppliances[transformHandle].Item2.TurnOff();
        }

        public TransformPair GetTransformPair(TransformHandle transformHandle)
        {
            return _assignedKitchenAppliances[transformHandle].Item1;
        }

        public void ReturnTransform(TransformHandle transformHandle)
        {
            _assignedKitchenAppliances[transformHandle].Item1.Release();
        }
    }
}