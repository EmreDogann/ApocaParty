using System;
using Actors;
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

    [Serializable]
    public class Message
    {
        public ActorSO actor;
        [TextArea(3, 5)]
        public string message;
    }
}