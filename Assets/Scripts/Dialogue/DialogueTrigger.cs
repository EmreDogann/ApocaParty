using System;
using MyBox;
using UnityEngine;

namespace Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        public Message[] messages;

        [ButtonMethod]
        public void StartDialogue()
        {
            DialogueManager.Instance.OpenDialogue(messages);
        }
    }

    // public enum ActorType
    // {
    //     CthUwU,
    //     Azathoth
    // }

    [Serializable]
    public class Message
    {
        public ActorSO actor;
        [TextArea(3, 5)]
        public string message;
    }
}