using MyBox;
using TMPro;
using UI.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public View dialogueView;
        public Image actorImage;
        public TextMeshProUGUI actorName;
        public TextMeshProUGUI messageText;
        public RectTransform backgroundBox;

        [Separator("Controls")]
        public InputActionReference confirmAction;

        private Message[] currentMessages;
        private int messageIndex;

        public static DialogueManager Instance;
        public static bool isActive;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OpenDialogue(Message[] messages)
        {
            currentMessages = messages;
            messageIndex = 0;
            isActive = true;

            dialogueView.Open();
            DisplayMessage();
        }

        private void DisplayMessage()
        {
            Message messageToDisplay = currentMessages[messageIndex];
            messageText.text = messageToDisplay.message;

            ActorSO actorToDisplay = messageToDisplay.actor;
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
        }

        public void NextMessage()
        {
            messageIndex++;
            if (messageIndex < currentMessages.Length)
            {
                DisplayMessage();
            }
            else
            {
                dialogueView.Close();
                isActive = false;
            }
        }

        private void Start() {}

        private void Update()
        {
            if (confirmAction.action.WasPressedThisFrame())
            {
                NextMessage();
            }
        }
    }
}