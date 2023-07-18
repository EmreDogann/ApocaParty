using MyBox;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        public Message[] messages;
        public TextAsset tempScript;

        [ButtonMethod]
        public void StartDialogue()
        {
            DialogueManager.Instance.OpenDialogue(Messages.ParseMessages(tempScript, DialogueManager.Instance.allActorSos));
            // DialogueManager.Instance.OpenDialogue(messages);
        }
    }
}