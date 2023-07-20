using System;
using MyBox;
using Needs;
using UnityEngine;
using Utils;

namespace PartyEvents
{
    public abstract class PartyEvent : MonoBehaviour
    {
        public static Action<NeedMetrics> OnNeedEvent;
        public static Action<int> OnMoodEvent;
        [SerializeField] protected bool triggerNeedEvent;
        [ConditionalField(nameof(triggerNeedEvent))] [MetricsRange(-100.0f, 100.0f)]
        [SerializeField] protected NeedMetrics Metrics;

        [SerializeField] protected bool triggerMoodEvent;
        [ConditionalField(nameof(triggerMoodEvent))] [Range(-4, 4)] [SerializeField] protected int moodPoints;

        public virtual void TriggerEvent()
        {
            if (triggerNeedEvent)
            {
                OnNeedEvent?.Invoke(Metrics);
            }

            if (triggerMoodEvent)
            {
                OnMoodEvent?.Invoke(moodPoints);
            }
        }
    }
}