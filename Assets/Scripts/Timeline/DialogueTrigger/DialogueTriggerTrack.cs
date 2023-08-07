using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.DialogueTrigger
{
    [TrackColor(0.2747864f, 0.5855064f, 0.8962264f)]
    [TrackClipType(typeof(DialogueTriggerClip))]
    public class DialogueTriggerTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<DialogueTriggerMixerBehaviour>.Create(graph, inputCount);
        }
    }
}