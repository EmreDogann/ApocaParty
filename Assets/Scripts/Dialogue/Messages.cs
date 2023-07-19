using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dialogue {
    public static class Messages {
        private const string ELIPSES = "...";
        private const int CHARACTER_LIMIT = 193;

        public static Message[] ParseMessages(string dialogueString, List<ActorSO> allActorsSOs) {
            List<Message> messages = new List<Message>();

            using (StringReader reader = new StringReader(dialogueString)) {
                ActorSO currentActor = null;
                string messageString = "";

                string line;
                while ((line = reader.ReadLine()) != null) {
                    Debug.Log("MessageParse: Line:" + line);

                    // Find the Actor for which the dialogue is attributed to 
                    if (line.StartsWith("@@")) {
                        // If there is a message with a new character, then store the current message with the old actor
                        if (currentActor != null) {
                            messages.AddRange(createMessageWrapToCharacterLimit(currentActor, messageString));

                            // Reset
                            currentActor = null;
                            messageString = "";
                        }

                        // Find the new Actor from the Dialogue file
                        string actorName = line.Replace("@@", "").Trim();
                        currentActor = allActorsSOs.Find(so => so.name.Equals(actorName));
                        Debug.Log("MessageParse: Name:" + currentActor.name);
                        continue; // If actor found, go to the next line
                    }

                    // Continue to next line until we have an actor
                    if (currentActor == null)
                        continue;

                    messageString += line;
                }

                if (currentActor != null) {
                    messages.AddRange(createMessageWrapToCharacterLimit(currentActor, messageString));
                }
            }


            return messages.ToArray();
        }

        public static Message[] ParseMessages(TextAsset dialogueTextAsset, List<ActorSO> allActorsSOs) {
            return ParseMessages(dialogueTextAsset.text, allActorsSOs);
        }

        private static List<Message> createMessageWrapToCharacterLimit(ActorSO currentActor, string messageString) {
            List<Message> messages = new List<Message>();

            if (messageString.Length < CHARACTER_LIMIT - ELIPSES.Length) {
                Message message = new Message(currentActor, messageString);
                messages.Add(message);
            }
            else {
                List<string> messegeStrings = new List<string>();
                while (messageString.Length > 0) {
                    // Get head of the text
                    string head = null;

                    if (CHARACTER_LIMIT - ELIPSES.Length < messageString.Length) {
                        head = messageString.Substring(0, CHARACTER_LIMIT - ELIPSES.Length);

                        int nextWhitespace = messageString.IndexOf(" ", head.Length);

                        if (nextWhitespace != -1)
                            head += messageString.Substring(head.Length, nextWhitespace - head.Length);

                        head += ELIPSES;
                    }
                    else {
                        head = messageString;
                        // messageString = messageString.Remove(0, head.Length);
                    }

                    // Create message with head
                    Message message = new Message(currentActor, head);
                    messages.Add(message);

                    // remove head
                    int amountToRemove = head.Length;
                    if (head.Length > CHARACTER_LIMIT && head.Length > ELIPSES.Length)
                        amountToRemove = head.Length - ELIPSES.Length;
                    messageString = messageString.Remove(0, amountToRemove);
                }
            }

            return messages;
        }
    }
}