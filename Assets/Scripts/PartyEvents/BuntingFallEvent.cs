using MyBox;

namespace PartyEvents
{
    public class BuntingFallEvent : PartyEvent
    {
        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
        }

        public override PartyEventType GetEventType()
        {
            return PartyEventType.BuntingFall;
        }
    }
}