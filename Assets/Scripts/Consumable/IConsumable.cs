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
        public Transform GetTransform();
        public ConsumedData Consume();
        public void Claim();
        public bool IsClaimed();
        public bool IsConsumed();
        public bool IsOnTable();
    }

    internal interface IConsumableInternal
    {
        void ResetConsumable();
    }
}