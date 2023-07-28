using System;
using Actors;
using UnityEngine;

namespace Dialogue
{
    [Serializable]
    public class Message
    {
        public Message(ActorSO actor, string message)
        {
            this.actor = actor;
            text = message;
        }

        public ActorSO actor;
        public bool playActorSound;

        [TextArea(3, 5)]
        public string text;
    }
}