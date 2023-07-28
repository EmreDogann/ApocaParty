using MyBox;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        public ConversationSO conversation;

        public UnityEvent OnDialogueFinished;

        [ButtonMethod]
        public void StartDialogue()
        {
            DialogueManager.Instance.OpenDialogue(conversation.messages, DialogueFinished);
        }

        private void DialogueFinished()
        {
            OnDialogueFinished?.Invoke();
        }
    }
}