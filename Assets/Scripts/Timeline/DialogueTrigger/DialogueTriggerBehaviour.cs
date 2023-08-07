using System;
using Dialogue;
using UnityEngine.Playables;

namespace Timeline.DialogueTrigger
{
    [Serializable]
    public class DialogueTriggerBehaviour : PlayableBehaviour
    {
        public ConversationSO dialogueConversation;
        public bool followProgress;
        public bool jumpToEnd;

        private PlayableGraph _graph;
        private Playable _thisPlayable;
        private bool _began;
        private float _startingTime;

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
                    _startingTime = (float)_graph.GetRootPlayable(0).GetTime();
                    DialogueManager.Instance.OpenDialogue(dialogueConversation.messages, OnDialogueEnd,
                        followProgress ? OnDialogueProgress : null);
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
            if (_graph.IsValid())
            {
                _graph.GetRootPlayable(0).SetSpeed(1);
                if (jumpToEnd)
                {
                    JumpToEndOfPlayable();
                }
            }
        }

        public void OnDialogueProgress(float progressPercentage)
        {
            if (_graph.IsValid())
            {
                _graph.GetRootPlayable(0)
                    .SetTime(_startingTime + (float)_thisPlayable.GetDuration() * progressPercentage);
            }
        }

        private void JumpToEndOfPlayable()
        {
            _graph.GetRootPlayable(0).SetTime(_startingTime + _thisPlayable.GetDuration());
        }
    }
}