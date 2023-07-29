using System;
using Guest;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.GuestNeed
{
    [Serializable]
    public class GuestNeedClip : PlayableAsset, ITimelineClipAsset
    {
        public GuestNeedBehaviour template = new GuestNeedBehaviour();
        public ExposedReference<GuestAI> guest;

        public ClipCaps clipCaps => ClipCaps.Extrapolation;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<GuestNeedBehaviour>.Create(graph, template);
            GuestNeedBehaviour clone = playable.GetBehaviour();
            clone.guest = guest.Resolve(graph.GetResolver());
            return playable;
        }
    }
}