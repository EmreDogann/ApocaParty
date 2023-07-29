using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.ActivateGuestAI
{
    [TrackColor(0.8018868f, 0.1021271f, 0.1224727f)]
    [TrackClipType(typeof(ActivateGuestAIClip))]
    public class ActivateGuestAITrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ActivateGuestAIMixerBehaviour>.Create(graph, inputCount);
        }
    }
}