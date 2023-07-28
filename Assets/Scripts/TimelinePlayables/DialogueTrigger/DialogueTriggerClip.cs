using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.DialogueTrigger
{
    [Serializable]
    public class DialogueTriggerClip : PlayableAsset, ITimelineClipAsset
    {
        public DialogueTriggerBehaviour template = new DialogueTriggerBehaviour();

        public ClipCaps clipCaps => ClipCaps.ClipIn;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DialogueTriggerBehaviour>.Create(graph, template);
            DialogueTriggerBehaviour clone = playable.GetBehaviour();
            return playable;
        }
    }
}