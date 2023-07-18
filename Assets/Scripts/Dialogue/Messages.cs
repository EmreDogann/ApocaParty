using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Dialogue {
    public static class Messages {
        private const string ELIPSES = "...";
        private const int CHARACTER_LIMIT = 344;

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
                            // Add to return list
                            // Check if current string is within the character limit, otherwise split it into multiple messages
                            // if (messageString.Length < CHARACTER_LIMIT - ELIPSES.Length) {
                            //     Message message = new Message(currentActor, messageString);
                            //     messages.Add(message);
                            // }
                            // else {
                            //     List<string> messegeStrings = new List<string>();
                            //     while (!messageString.Length.Equals("")) {
                            //         // Get head of the text
                            //         string head = null;
                            //
                            //         if (CHARACTER_LIMIT - ELIPSES.Length < messageString.Length) {
                            //             head = messageString.Substring(0, CHARACTER_LIMIT - ELIPSES.Length) + ELIPSES;
                            //         }
                            //         else {
                            //             head = messageString.Substring(0, messageString.Length);
                            //         }
                            //
                            //         // Create message with head
                            //         Message message = new Message(currentActor, head);
                            //         messages.Add(message);
                            //
                            //         // remove head
                            //         messageString = messageString.Remove(0, head.Length);
                            //     }
                            // }
                            messages.AddRange(createMessageWrapToCharacterLimit(currentActor, messageString));

                            // Reset
                            currentActor = null;
                            messageString = "";
                        }

                        // Find the new Actor from the Dialogue file
                        string actorName = line.Replace("@@", "").Trim();
                        currentActor = allActorsSOs.Find(so => so.name.Equals(actorName));
                        continue; // If actor found, go to the next line
                        Debug.Log("MessageParse: Name:" + currentActor.name);
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
                while (!messageString.Length.Equals("")) {
                    // Get head of the text
                    string head = null;

                    if (CHARACTER_LIMIT - ELIPSES.Length < messageString.Length) {
                        head = messageString.Substring(0, CHARACTER_LIMIT - ELIPSES.Length) + ELIPSES;
                    }
                    else {
                        head = messageString.Substring(0, messageString.Length);
                    }

                    // Create message with head
                    Message message = new Message(currentActor, head);
                    messages.Add(message);

                    // remove head
                    messageString = messageString.Remove(0, head.Length);
                }
            }

            return messages;
        }
        
    }
}