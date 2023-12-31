using Dialogue;
using UnityEngine.Playables;

namespace Timeline.DialogueTrigger
{
    public class DialogueTriggerMixerBehaviour : PlayableBehaviour
    {
        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            DialogueManager trackBinding = playerData as DialogueManager;

            if (!trackBinding)
            {
                return;
            }

            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                var inputPlayable = (ScriptPlayable<DialogueTriggerBehaviour>)playable.GetInput(i);
                DialogueTriggerBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.
            }
        }
    }
}