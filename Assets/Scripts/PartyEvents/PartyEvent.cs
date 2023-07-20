using System;
using Needs;
using UnityEngine;
using Utils;

namespace PartyEvents
{
    public enum PartyEventType
    {
        FamineAtDrinks,
        MusicPlaying,
        MusicMachineBreaks,
        FoodBurning,
        PowerOutage,
        BuntingFall
    }

    [Serializable]
    public class PartyEventData
    {
        public PartyEventType eventType;
        public NeedMetrics needsCost;
        public int moodCost;
    }

    public abstract class PartyEvent : MonoBehaviour
    {
        public static Action<PartyEventData> OnPartyEvent;
        [MetricsRange(-1.0f, 1.0f)] [SerializeField] protected NeedMetrics Metrics;
        [Range(-4, 4)] [SerializeField] protected int moodPoints;

        public virtual void TriggerEvent()
        {
            PartyEventData partyEventData = new PartyEventData
            {
                eventType = GetEventType(),
                needsCost = Metrics,
                moodCost = moodPoints
            };

            OnPartyEvent?.Invoke(partyEventData);
        }

        public abstract PartyEventType GetEventType();
    }
}