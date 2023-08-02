using System;
using Needs;
using UnityEngine;
using Utils;

namespace Consumable
{
    [Serializable]
    public class ConsumedData
    {
        public NeedType needType;
        [Range(-4, 4)] public int moodPoints;
        [MetricsRange(-1.0f, 1.0f)] public NeedMetrics needMetrics;
    }

    public interface IConsumable
    {
        public void SetSorting(int layer, int order);
        public Transform GetTransform();
        public ConsumedData Consume();
        public void Spill();
        public bool IsSpilled();
        public void Cleanup();
        public void Claim();
        public bool IsConsumed();
        public bool IsAvailable();
    }

    internal interface IConsumableInternal
    {
        void ResetConsumable();
    }
}