using System;
using System.Collections;
using System.Text;
using Actors;
using Events;
using MyBox;
using TMPro;
using UI;
using UI.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dialogue
{
    [RequireComponent(typeof(BoolEventListener))]
    public class DialogueManager : MonoBehaviour
    {
        public View dialogueView;
        public Image actorImage;
        public TextMeshProUGUI actorName;
        public TextMeshProUGUI messageText;
        public RectTransform backgroundBox;
        [SerializeField] private float animationSpeed = 0.05f;

        [Separator("Controls")]
        public InputActionReference confirmAction;

        private Message[] _currentMessages;
        private int _messageIndex;

        public static DialogueManager Instance;
        public bool DialogueIsPlaying { get; private set; }

        private BoolEventListener _listener;
        private Coroutine animationCoroutine;

        private Action callback;

        private readonly string[] connectives =
        {
            "also, ",
            "and "
        };

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

        public void OpenDialogue(Message[] messages)
        {
            _currentMessages = messages;
            _messageIndex = 0;
            _listener.Event.Raise(true);
            DialogueIsPlaying = true;

            UIManager.Instance.Show(dialogueView);
            DisplayMessage();
        }

        public void OpenRandomDialogue(Message[] messages, Action callbackAction)
        {
            callback = callbackAction;
            for (int i = 1; i < messages.Length; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(messages[i].text);
                stringBuilder[0] = char.ToLower(stringBuilder[0]);
                stringBuilder.Insert(0, connectives[Random.Range(0, connectives.Length)]);

                messages[i].text = stringBuilder.ToString();
            }

            _currentMessages = messages;
            _messageIndex = 0;
            _listener.Event.Raise(true);
            DialogueIsPlaying = true;

            UIManager.Instance.Show(dialogueView);
            animationCoroutine = StartCoroutine(DisplayMessage());
        }


        private IEnumerator DisplayMessage()
        {
            ActorSO actorToDisplay = _currentMessages[_messageIndex].actor;
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
            messageText.text = string.Empty;
            foreach (char c in _currentMessages[_messageIndex].text)
            {
                messageText.text += c;
                yield return new WaitForSecondsRealtime(animationSpeed);
            }
        }

        private void DisplayEntireMessage()
        {
            messageText.text = string.Empty;
            Message messageToDisplay = _currentMessages[_messageIndex];
            messageText.text = messageToDisplay.text;

            ActorSO actorToDisplay = messageToDisplay.actor;
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
        }

        public void NextMessage()
        {
            _messageIndex++;
            if (_messageIndex < _currentMessages.Length)
            {
                animationCoroutine = StartCoroutine(DisplayMessage());
            }
            else
            {
                StartCoroutine(ExitDialogue());
            }
        }

        private IEnumerator ExitDialogue()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            UIManager.Instance.Back();
            DialogueIsPlaying = false;
            _listener.Event.Raise(false);

            callback?.Invoke();
        }

        private void Update()
        {
            if (confirmAction.action.WasPressedThisFrame() && UIManager.Instance.GetCurrentView() == dialogueView)
            {
                if (_messageIndex >= _currentMessages.Length)
                {
                    return;
                }

                if (messageText.text != _currentMessages[_messageIndex].text)
                {
                    if (animationCoroutine != null)
                    {
                        StopCoroutine(animationCoroutine);
                    }

                    DisplayEntireMessage();
                }
                else
                {
                    NextMessage();
                }
            }
        }
    }
}