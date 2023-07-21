using MyBox;

namespace PartyEvents
{
    public class MusicPlayEvent : PartyEvent
    {
        [ButtonMethod]
        public override void TriggerEvent()
        {
            base.TriggerEvent();
        }

        public override PartyEventType GetEventType()
        {
            return PartyEventType.MusicPlaying;
        }
    }
}