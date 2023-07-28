using System;
using UnityEngine.Playables;
using Utils;

namespace TimelinePlayables.SceneLoader
{
    [Serializable]
    public class SceneLoaderBehaviour : PlayableBehaviour
    {
        public SceneReference sceneToLoad;

        private Playable _thisPlayable;

        public override void OnPlayableCreate(Playable playable)
        {
            _thisPlayable = playable;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (sceneToLoad != null)
            {
                if (SceneLoaderManager.Instance)
                {
                    SceneLoaderManager.Instance.SwapActiveScene(sceneToLoad.ScenePath);
                    JumpToEndOfPlayable();
                }
            }
        }

        private void JumpToEndOfPlayable()
        {
            _thisPlayable.SetTime(_thisPlayable.GetDuration());
        }
    }
}