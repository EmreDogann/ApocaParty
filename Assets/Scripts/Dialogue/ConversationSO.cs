using System;
using System.Collections.Generic;
using Actors;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue Conversation", menuName = "Dialogue/New Conversation", order = 0)]
    public class ConversationSO : ScriptableObject
    {
        public TextAsset script;
        public Message[] messages;
        private List<ActorSO> _allActors;

#if UNITY_EDITOR
        [ButtonMethod]
        private void ParseScript()
        {
            _allActors = GetAllActorInstances();
            messages = Messages.ParseMessages(script, _allActors);
        }
#endif

        [ButtonMethod]
        private void ClearParsedScript()
        {
            messages = Array.Empty<Message>();
        }

#if UNITY_EDITOR
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
#endif
    }
}