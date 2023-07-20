using System;
using MyBox;

namespace PartyEvents
{
    public class FamineEvent : PartyEvent
    {
        public static Action<int> OnFamineEvent;

        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
            OnFamineEvent?.Invoke(moodPoints);
        }
    }
}