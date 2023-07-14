using System.Collections;
using Events.UnityEvents;
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

        private Message[] _currentMessages;
        private int _messageIndex;

        public static DialogueManager Instance;
        public bool DialogueIsPlaying { get; private set; }
        private bool _dialogueIsPaused;

        private BoolEventListener _listener;

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

            _listener = GetComponent<BoolEventListener>();
        }

        private void OnEnable()
        {
            _listener.Response.AddListener(OnGamePaused);
        }

        private void OnDisable()
        {
            _listener.Response.RemoveListener(OnGamePaused);
        }

        public void OpenDialogue(Message[] messages)
        {
            _currentMessages = messages;
            _messageIndex = 0;
            DialogueIsPlaying = true;
            Time.timeScale = 0.0f;

            dialogueView.Open();
            DisplayMessage();
        }

        private void DisplayMessage()
        {
            Message messageToDisplay = _currentMessages[_messageIndex];
            messageText.text = messageToDisplay.message;

            ActorSO actorToDisplay = messageToDisplay.actor;
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
        }

        public void NextMessage()
        {
            _messageIndex++;
            if (_messageIndex < _currentMessages.Length)
            {
                DisplayMessage();
            }
            else
            {
                StartCoroutine(ExitDialogue());
            }
        }

        private IEnumerator ExitDialogue()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            dialogueView.Close();
            Time.timeScale = 1.0f;
            DialogueIsPlaying = false;
        }

        private void OnGamePaused(bool isPaused)
        {
            if (!DialogueIsPlaying)
            {
                return;
            }

            if (isPaused)
            {
                _dialogueIsPaused = true;
            }
            else
            {
                _dialogueIsPaused = false;
            }
        }

        private void Update()
        {
            if (confirmAction.action.WasPressedThisFrame() && DialogueIsPlaying && !_dialogueIsPaused)
            {
                NextMessage();
            }
        }
    }
}