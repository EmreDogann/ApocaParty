using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.ToggleInteractable
{
    [TrackColor(0.5795726f, 0.4389462f, 0.6792453f)]
    [TrackClipType(typeof(ToggleInteractableClip))]
    public class ToggleInteractableTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<ToggleInteractableMixerBehaviour>.Create(graph, inputCount);
        }
    }
}