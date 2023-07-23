using MyBox;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        public ConversationSO conversation;

        [ButtonMethod]
        public void StartDialogue()
        {
            DialogueManager.Instance.OpenDialogue(conversation.messages);
        }
    }
}