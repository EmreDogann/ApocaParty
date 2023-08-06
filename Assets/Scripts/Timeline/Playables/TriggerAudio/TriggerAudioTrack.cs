using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Playables.TriggerAudio
{
    [TrackColor(0.764151f, 0.2703364f, 0.4398309f)]
    [TrackClipType(typeof(TriggerAudioClip))]
    public class TriggerAudioTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TriggerAudioMixerBehaviour>.Create(graph, inputCount);
        }
    }
}