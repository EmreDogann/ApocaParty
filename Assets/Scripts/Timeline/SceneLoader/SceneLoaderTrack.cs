using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.SceneLoader
{
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(SceneLoaderClip))]
    public class SceneLoaderTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<SceneLoaderMixerBehaviour>.Create(graph, inputCount);
        }
    }
}