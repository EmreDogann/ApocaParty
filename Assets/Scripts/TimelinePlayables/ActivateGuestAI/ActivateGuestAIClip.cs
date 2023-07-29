using System;
using Guest;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.ActivateGuestAI
{
    [Serializable]
    public class ActivateGuestAIClip : PlayableAsset, ITimelineClipAsset
    {
        public ActivateGuestAIBehaviour template = new ActivateGuestAIBehaviour();
        public ExposedReference<GuestAI> guestAI;

        public ClipCaps clipCaps => ClipCaps.ClipIn;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ActivateGuestAIBehaviour>.Create(graph, template);
            ActivateGuestAIBehaviour clone = playable.GetBehaviour();
            clone.guestAI = guestAI.Resolve(graph.GetResolver());
            return playable;
        }
    }
}