using System;
using System.Collections.Generic;
using Actors;
using MyBox;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Random Dialogue Conversation", menuName = "Dialogue/New Random Conversation",
        order = 1)]
    public class RandomConversationSO : ScriptableObject
    {
        public TextAsset script;
        public Message[] messagesToRandomlyPick;
        private List<ActorSO> _allActors;

        [ButtonMethod]
        private void ParseScript()
        {
            _allActors = GetAllActorInstances();
            messagesToRandomlyPick = Messages.ParseMessages(script, _allActors);
        }

        [ButtonMethod]
        private void ClearParsedScript()
        {
            messagesToRandomlyPick = Array.Empty<Message>();
        }

        public Message GetRandomMessage()
        {
            Message message = messagesToRandomlyPick[Random.Range(0, messagesToRandomlyPick.Length)];
            return new Message(message.actor, message.text);
        }

        private List<ActorSO> GetAllActorInstances()
        {
            string[] guids =
                AssetDatabase.FindAssets("t:" + nameof(ActorSO), new[] { "Assets/ScriptableObjects/Actors" });
            var a = new List<ActorSO>(guids.Length);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                a.Add(AssetDatabase.LoadAssetAtPath<ActorSO>(path));
            }

            return a;
        }
    }
}