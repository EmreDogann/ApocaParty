using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.ChangeSortOrder
{
    [Serializable]
    public class ChangeSortOrderClip : PlayableAsset, ITimelineClipAsset
    {
        public ChangeSortOrderBehaviour template = new ChangeSortOrderBehaviour();
        public ExposedReference<SpriteRenderer> spriteRenderer;

        public ClipCaps clipCaps => ClipCaps.ClipIn;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChangeSortOrderBehaviour>.Create(graph, template);
            ChangeSortOrderBehaviour clone = playable.GetBehaviour();
            clone.spriteRenderer = spriteRenderer.Resolve(graph.GetResolver());
            return playable;
        }
    }
}