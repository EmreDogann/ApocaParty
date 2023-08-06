using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Playables.FocusSprite
{
    [TrackColor(0.1417319f, 0.2359603f, 0.3301887f)]
    [TrackClipType(typeof(FocusSpriteClip))]
    public class FocusSpriteTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<FocusSpriteMixerBehaviour>.Create(graph, inputCount);
        }
    }
}