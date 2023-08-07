using System;
using Guest;
using MyBox;
using Needs;
using UnityEngine.Playables;

namespace Timeline.GuestNeed
{
    public enum TimelineGuestNeedAction
    {
        AddNeed,
        RemoveNeed,
        ExpireNeed,
        FulfillNeed,
        PauseNeed,
        UnpauseNeed
    }

    [Serializable]
    public class GuestNeedBehaviour : PlayableBehaviour
    {
        public GuestAI guest;
        public TimelineGuestNeedAction needAction;
        public NeedType needType;

        [ConditionalField(nameof(needAction), false, TimelineGuestNeedAction.AddNeed)]
        public bool pauseOnAdd;
        [ConditionalField(nameof(needAction), false, TimelineGuestNeedAction.AddNeed)]
        public bool startAsResolved;
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
                    if (startAsResolved)
                    {
                        guest.needSystem.TryAddNeed(needType, startAsResolved, startingExpirationTime);
                    }
                    else
                    {
                        guest.needSystem.TryAddNeed(needType, startingExpirationTime);
                    }

                    if (pauseOnAdd)
                    {
                        guest.needSystem.TryPauseNeed(needType);
                    }

                    break;
                case TimelineGuestNeedAction.RemoveNeed:
                    guest.needSystem.TryRemoveNeed(needType);
                    break;
                case TimelineGuestNeedAction.ExpireNeed:
                    guest.needSystem.TryExpireNeed(needType);
                    break;
                case TimelineGuestNeedAction.PauseNeed:
                    guest.needSystem.TryPauseNeed(needType);
                    break;
                case TimelineGuestNeedAction.UnpauseNeed:
                    guest.needSystem.TryUnpauseNeed(needType);
                    break;
                case TimelineGuestNeedAction.FulfillNeed:
                    guest.needSystem.TryFulfillNeed(needType, rewardMetric, moodReward);
                    break;
            }
        }
    }
}