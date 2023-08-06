using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Playables.TriggerAudio
{
    [Serializable]
    public class TriggerAudioClip : PlayableAsset, ITimelineClipAsset
    {
        public TriggerAudioBehaviour template = new TriggerAudioBehaviour();
        public ExposedReference<Transform> triggerPosition;

        public ClipCaps clipCaps => ClipCaps.ClipIn;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TriggerAudioBehaviour>.Create(graph, template);
            TriggerAudioBehaviour clone = playable.GetBehaviour();
            clone.triggerPosition = triggerPosition.Resolve(graph.GetResolver());
            return playable;
        }
    }
}