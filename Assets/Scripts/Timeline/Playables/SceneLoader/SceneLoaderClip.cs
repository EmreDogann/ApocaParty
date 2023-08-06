using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Playables.SceneLoader
{
    [Serializable]
    public class SceneLoaderClip : PlayableAsset, ITimelineClipAsset
    {
        public SceneLoaderBehaviour template = new SceneLoaderBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SceneLoaderBehaviour>.Create(graph, template);
            SceneLoaderBehaviour clone = playable.GetBehaviour();
            return playable;
        }
    }
}