using System;
using System.Collections;
using System.Text;
using Actors;
using Audio;
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
        public InputActionReference keyboardConfirmAction;
        public InputActionReference mouseConfirmAction;

        [Separator("Other")]
        public AudioSO confirmAudio;
        [SerializeField] private bool useUIViewSystem;

        private Message[] _currentMessages;
        private int _messageIndex;

        public static DialogueManager Instance;
        public bool DialogueIsPlaying { get; private set; }

        private BoolEventListener _listener;
        private Coroutine _animationCoroutine;

        private Action _callback;

        private readonly string[] _connectives =
        {
            "also, ",
            "and "
        };

        private bool canContinueToNextLine;

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

        public void OpenDialogue(Message[] messages, Action callbackAction = null)
        {
            _callback = callbackAction;

            _currentMessages = messages;
            _messageIndex = 0;
            DialogueIsPlaying = true;

            if (useUIViewSystem)
            {
                UIManager.Instance.Show(dialogueView);
            }

            _animationCoroutine = StartCoroutine(DisplayMessage());
            _listener.Event.Raise(true);
        }

        public void OpenRandomDialogue(Message[] messages, Action callbackAction)
        {
            _callback = callbackAction;
            for (int i = 1; i < messages.Length; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(messages[i].text);
                stringBuilder[0] = char.ToLower(stringBuilder[0]);
                stringBuilder.Insert(0, _connectives[Random.Range(0, _connectives.Length)]);

                messages[i].text = stringBuilder.ToString();
            }

            _currentMessages = messages;
            _messageIndex = 0;
            DialogueIsPlaying = true;

            if (useUIViewSystem)
            {
                UIManager.Instance.Show(dialogueView);
            }

            _animationCoroutine = StartCoroutine(DisplayMessage());
            _listener.Event.Raise(true);
        }


        private IEnumerator DisplayMessage()
        {
            canContinueToNextLine = false;

            ActorSO actorToDisplay = _currentMessages[_messageIndex].actor;
            if (actorToDisplay)
            {
                actorName.text = actorToDisplay.name;
                actorImage.sprite = actorToDisplay.sprite ? actorToDisplay.sprite : null;

                if (_currentMessages[_messageIndex].playActorSound)
                {
                    actorToDisplay.voice.Play2D();
                }

                Color color = actorImage.color;
                color.a = 1;
                actorImage.color = color;
            }
            else
            {
                actorName.text = "";
                actorImage.sprite = null;

                Color color = actorImage.color;
                color.a = 0;
                actorImage.color = color;
            }

            messageText.text = _currentMessages[_messageIndex].text;
            messageText.maxVisibleCharacters = 0;

            bool isAddingRichTextTag = false;
            foreach (char letter in messageText.text)
            {
                while (useUIViewSystem && UIManager.Instance.GetCurrentView() != dialogueView)
                {
                    yield return null;
                }

                if (messageText.maxVisibleCharacters == _currentMessages[_messageIndex].text.Length)
                {
                    break;
                }

                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    if (letter == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                else
                {
                    messageText.maxVisibleCharacters++;
                    yield return new WaitForSecondsRealtime(animationSpeed);
                }
            }

            canContinueToNextLine = true;
        }

        public void NextMessage()
        {
            _messageIndex++;
            if (_messageIndex < _currentMessages.Length)
            {
                if (_animationCoroutine != null)
                {
                    StopCoroutine(_animationCoroutine);
                }

                _animationCoroutine = StartCoroutine(DisplayMessage());
            }
            else
            {
                StartCoroutine(ExitDialogue());
            }
        }

        private IEnumerator ExitDialogue()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            if (useUIViewSystem)
            {
                UIManager.Instance.Back();
            }

            DialogueIsPlaying = false;
            _listener.Event.Raise(false);

            _callback?.Invoke();
            _callback = null;
        }

        private void Update()
        {
            if (useUIViewSystem && UIManager.Instance.GetCurrentView() != dialogueView)
            {
                return;
            }

            if (keyboardConfirmAction.action.WasPressedThisFrame() || mouseConfirmAction.action.WasPressedThisFrame())
            {
                if (canContinueToNextLine)
                {
                    confirmAudio.Play2D();
                    if (_messageIndex >= _currentMessages.Length)
                    {
                        return;
                    }

                    NextMessage();
                }
                else
                {
                    messageText.maxVisibleCharacters = _currentMessages[_messageIndex].text.Length;
                }
            }
        }
    }
}