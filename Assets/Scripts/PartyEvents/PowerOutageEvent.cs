using MyBox;

namespace PartyEvents
{
    public class PowerOutageEvent : PartyEvent
    {
        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
        }

        public override PartyEventType GetEventType()
        {
            return PartyEventType.PowerOutage;
        }
    }
}