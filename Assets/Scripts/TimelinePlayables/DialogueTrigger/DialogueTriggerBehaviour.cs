using System;
using Dialogue;
using UnityEngine.Playables;

namespace TimelinePlayables.DialogueTrigger
{
    [Serializable]
    public class DialogueTriggerBehaviour : PlayableBehaviour
    {
        public ConversationSO dialogueConversation;
        public bool jumpToEnd;

        private PlayableGraph _graph;
        private Playable _thisPlayable;
        private bool _began;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
            _began = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (dialogueConversation != null && !_began)
            {
                if (DialogueManager.Instance)
                {
                    //Pause without breaking the current animation states. Work around for 2018.2
                    _graph.GetRootPlayable(0).SetSpeed(0);
                    DialogueManager.Instance.OpenDialogue(dialogueConversation.messages, OnDialogueEnd);
                    _began = true;
                }
                else
                {
                    if (jumpToEnd)
                    {
                        JumpToEndOfPlayable();
                    }

                    _graph.GetRootPlayable(0).SetSpeed(1);
                }
            }
        }

        public void OnDialogueEnd()
        {
            //Unpause
            _graph.GetRootPlayable(0).SetSpeed(1);
            if (jumpToEnd)
            {
                JumpToEndOfPlayable();
            }
        }

        private void JumpToEndOfPlayable()
        {
            _graph.GetRootPlayable(0).SetTime(_graph.GetRootPlayable(0).GetTime() + _thisPlayable.GetDuration());
        }
    }
}