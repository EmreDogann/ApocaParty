using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.GuestNeed
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(GuestNeedClip))]
    public class GuestNeedTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<GuestNeedMixerBehaviour>.Create(graph, inputCount);
        }
    }
}