using UnityEngine.Playables;

namespace Timeline.SceneLoader
{
    public class SceneLoaderMixerBehaviour : PlayableBehaviour
    {
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<SceneLoaderBehaviour>)playable.GetInput(i);
                SceneLoaderBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.
            }
        }
    }
}