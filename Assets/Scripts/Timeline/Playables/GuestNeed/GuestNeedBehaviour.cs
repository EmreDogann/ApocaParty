using System;
using Guest;
using MyBox;
using Needs;
using UnityEngine.Playables;

namespace Timeline.Playables.GuestNeed
{
    public enum TimelineGuestNeedAction
    {
        AddNeed,
        RemoveNeed,
        FulfillNeed
    }

    [Serializable]
    public class GuestNeedBehaviour : PlayableBehaviour
    {
        public GuestAI guest;
        public TimelineGuestNeedAction needAction;
        public NeedType needType;

        [ConditionalField(nameof(needAction), false, TimelineGuestNeedAction.AddNeed)]
        public float startingExpirationTime;

        [ConditionalField(nameof(needAction), false, TimelineGuestNeedAction.FulfillNeed)]
        public NeedMetrics rewardMetric;
        [ConditionalField(nameof(needAction), false, TimelineGuestNeedAction.FulfillNeed)] public int moodReward;

        private PlayableGraph _graph;
        private Playable _thisPlayable;
        private bool _began;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
            _began = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (_began)
            {
                return;
            }

            _began = true;

            if (guest.needSystem == null)
            {
                return;
            }

            switch (needAction)
            {
                case TimelineGuestNeedAction.AddNeed:
                    guest.needSystem.TryAddNeed(needType, startingExpirationTime);
                    break;
                case TimelineGuestNeedAction.RemoveNeed:
                    guest.needSystem.TryRemoveNeed(needType);
                    break;
                case TimelineGuestNeedAction.FulfillNeed:
                    guest.needSystem.TryFulfillNeed(needType, rewardMetric, moodReward);
                    break;
            }
        }
    }
}