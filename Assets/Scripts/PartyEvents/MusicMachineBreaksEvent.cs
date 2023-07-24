using MyBox;

namespace PartyEvents
{
    public class MusicMachineBreaksEvent : PartyEvent
    {
        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
        }

        public override PartyEventType GetEventType()
        {
            return PartyEventType.MusicMachineBreaks;
        }
    }
}