using UnityEngine.Playables;

namespace Timeline.Playables.ChangeSortOrder
{
    public class ChangeSortOrderMixerBehaviour : PlayableBehaviour
    {
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<ChangeSortOrderBehaviour>)playable.GetInput(i);
                ChangeSortOrderBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.
            }
        }
    }
}