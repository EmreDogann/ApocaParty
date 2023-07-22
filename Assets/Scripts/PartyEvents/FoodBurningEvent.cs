using MyBox;

namespace PartyEvents
{
    public class FoodBurningEvent : PartyEvent
    {
        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
        }

        public override PartyEventType GetEventType()
        {
            return PartyEventType.FoodBurning;
        }
    }
}