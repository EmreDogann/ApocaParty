using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Playables.ChangeSortOrder
{
    [TrackColor(0.5543827f, 0.2202741f, 0.6226415f)]
    [TrackClipType(typeof(ChangeSortOrderClip))]
    public class ChangeSortOrderTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ChangeSortOrderMixerBehaviour>.Create(graph, inputCount);
        }
    }
}