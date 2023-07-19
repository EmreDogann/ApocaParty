using System;
using UnityEngine;

namespace Dialogue {
    [Serializable]
    public class Message {
        public Message(ActorSO actor, string message) {
            this.actor = actor;
            this.message = message;
        }

        public ActorSO actor;

        [TextArea(3, 5)]
        public string message;
    }
}