using System.Collections.Generic;
using System.IO;
using Actors;
using UnityEngine;

namespace Dialogue
{
    public static class Messages
    {
        private const string ELIPSES = "...";
        private const int CHARACTER_LIMIT = 193;

        public static Message[] ParseMessages(string dialogueString, List<ActorSO> allActorsSOs)
        {
            var messages = new List<Message>();

            using (StringReader reader = new StringReader(dialogueString))
            {
                ActorSO currentActor = null;
                string messageString = "";

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Debug.Log("MessageParse: Line:" + line);

                    // Find the Actor for which the dialogue is attributed to 
                    if (line.StartsWith("@@"))
                    {
                        // If there is a message with a new character, then store the current message with the old actor
                        if (currentActor != null)
                        {
                            messages.AddRange(createMessageWrapToCharacterLimit(currentActor, messageString));

                            // Reset
                            currentActor = null;
                            messageString = "";
                        }

                        // Find the new Actor from the Dialogue file
                        string actorName = line.Replace("@@", "").Trim();
                        currentActor = allActorsSOs.Find(so => so.name.Equals(actorName));
                        // Debug.Log("MessageParse: Name:" + currentActor.name);
                        continue; // If actor found, go to the next line
                    }

                    // Continue to next line until we have an actor
                    if (currentActor == null)
                    {
                        continue;
                    }

                    messageString += line;
                }

                if (currentActor != null)
                {
                    messages.AddRange(createMessageWrapToCharacterLimit(currentActor, messageString));
                }
            }


            return messages.ToArray();
        }

        public static Message[] ParseMessages(TextAsset dialogueTextAsset, List<ActorSO> allActorsSOs)
        {
            string dialogueText = dialogueTextAsset.text;
            // Remove replace characters
            if (dialogueText.IndexOf('\u2013') > -1)
            {
                dialogueText = dialogueText.Replace('\u2013', '-'); // en dash
            }

            if (dialogueText.IndexOf('\u2014') > -1)
            {
                dialogueText = dialogueText.Replace('\u2014', '-'); // em dash
            }

            if (dialogueText.IndexOf('\u2015') > -1)
            {
                dialogueText = dialogueText.Replace('\u2015', '-'); // horizontal bar
            }

            if (dialogueText.IndexOf('\u2017') > -1)
            {
                dialogueText = dialogueText.Replace('\u2017', '_'); // double low line
            }

            if (dialogueText.IndexOf('\u2018') > -1)
            {
                dialogueText = dialogueText.Replace('\u2018', '\''); // left single quotation mark
            }

            if (dialogueText.IndexOf('\u2019') > -1)
            {
                dialogueText = dialogueText.Replace('\u2019', '\''); // right single quotation mark
            }

            if (dialogueText.IndexOf('\u201a') > -1)
            {
                dialogueText = dialogueText.Replace('\u201a', ','); // single low-9 quotation mark
            }

            if (dialogueText.IndexOf('\u201b') > -1)
            {
                dialogueText = dialogueText.Replace('\u201b', '\''); // single high-reversed-9 quotation mark
            }

            if (dialogueText.IndexOf('\u201c') > -1)
            {
                dialogueText = dialogueText.Replace('\u201c', '\"'); // left double quotation mark
            }

            if (dialogueText.IndexOf('\u201d') > -1)
            {
                dialogueText = dialogueText.Replace('\u201d', '\"'); // right double quotation mark
            }

            if (dialogueText.IndexOf('\u201e') > -1)
            {
                dialogueText = dialogueText.Replace('\u201e', '\"'); // double low-9 quotation mark
            }

            if (dialogueText.IndexOf('\u2026') > -1)
            {
                dialogueText = dialogueText.Replace("\u2026", "..."); // horizontal ellipsis
            }

            if (dialogueText.IndexOf('\u2032') > -1)
            {
                dialogueText = dialogueText.Replace('\u2032', '\''); // prime
            }

            if (dialogueText.IndexOf('\u2033') > -1)
            {
                dialogueText = dialogueText.Replace('\u2033', '\"'); // double prime
            }

            return ParseMessages(dialogueText, allActorsSOs);
        }

        private static List<Message> createMessageWrapToCharacterLimit(ActorSO currentActor, string messageString)
        {
            var messages = new List<Message>();

            if (messageString.Length < CHARACTER_LIMIT - ELIPSES.Length)
            {
                Message message = new Message(currentActor, messageString);
                messages.Add(message);
            }
            else
            {
                var messegeStrings = new List<string>();
                while (messageString.Length > 0)
                {
                    // Get head of the text
                    string head = null;

                    if (CHARACTER_LIMIT - ELIPSES.Length < messageString.Length)
                    {
                        head = messageString.Substring(0, CHARACTER_LIMIT - ELIPSES.Length);

                        int nextWhitespace = messageString.IndexOf(" ", head.Length);

                        if (nextWhitespace != -1)
                        {
                            head += messageString.Substring(head.Length, nextWhitespace - head.Length);
                        }

                        head += ELIPSES;
                    }
                    else
                    {
                        head = messageString;
                        // messageString = messageString.Remove(0, head.Length);
                    }

                    // Create message with head
                    Message message = new Message(currentActor, head);
                    messages.Add(message);

                    // remove head
                    int amountToRemove = head.Length;
                    if (head.Length > CHARACTER_LIMIT && head.Length > ELIPSES.Length)
                    {
                        amountToRemove = head.Length - ELIPSES.Length;
                    }

                    messageString = messageString.Remove(0, amountToRemove);
                }
            }

            return messages;
        }
    }
}