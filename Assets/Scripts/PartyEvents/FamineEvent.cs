using MyBox;

namespace PartyEvents
{
    public class FamineEvent : PartyEvent
    {
        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
        }

        public override PartyEventType GetEventType()
        {
            return PartyEventType.FamineAtDrinks;
        }
    }
}