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
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Dialogue
{
    [RequireComponent(typeof(BoolEventListener))]
    public class DialogueManager : MonoBehaviour
    {
        [Separator("UI Components")]
        public View dialogueView;
        public Image actorImage;
        public GameObject continueIcon;
        public TextMeshProUGUI actorName;
        public TextMeshProUGUI messageText;
        [SerializeField] private float animationSpeed = 0.05f;

        [Separator("Other")]
        public AudioSO confirmAudio;
        [SerializeField] private bool useUIViewSystem;

        private Message[] _currentMessages;
        private int _messageIndex;

        public static DialogueManager Instance;
        public bool DialogueIsPlaying { get; private set; }

        private BoolEventListener _listener;
        private Coroutine _animationCoroutine;

        private Action _onEndCallback;
        private Action<float> _progressCallback;

        private InputSystemUIInputModule _uiInputModule;
        private InputAction _keyboardConfirmAction;
        private InputAction _mouseConfirmAction;

        private readonly string[] _connectives =
        {
            "also, ",
            "and "
        };

        private bool _canContinueToNextLine;

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

            _uiInputModule = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<InputSystemUIInputModule>();

            if (_uiInputModule != null)
            {
                _keyboardConfirmAction = _uiInputModule.submit.action;
                _mouseConfirmAction = _uiInputModule.leftClick.action;
            }
        }

        public void SetDialogueSpeed(float speed)
        {
            animationSpeed = 1 - speed;
        }

        public void OpenDialogue(Message[] messages, Action onEndCallback = null, Action<float> progressCallback = null)
        {
            _onEndCallback = onEndCallback;
            _progressCallback = progressCallback;

            _currentMessages = messages;
            _messageIndex = 0;
            continueIcon.SetActive(false);
            DialogueIsPlaying = true;

            if (useUIViewSystem)
            {
                UIManager.Instance.Show(dialogueView);
            }

            _animationCoroutine = StartCoroutine(DisplayMessage());
            _listener.Event.Raise(true);
        }

        public void OpenRandomDialogue(Message[] messages, Action onEndCallback = null,
            Action<float> progressCallback = null)
        {
            _onEndCallback = onEndCallback;
            _progressCallback = progressCallback;

            for (int i = 1; i < messages.Length; i++)
            {
                StringBuilder stringBuilder = new StringBuilder(messages[i].text);
                stringBuilder[0] = char.ToLower(stringBuilder[0]);
                stringBuilder.Insert(0, _connectives[Random.Range(0, _connectives.Length)]);

                messages[i].text = stringBuilder.ToString();
            }

            _currentMessages = messages;
            _messageIndex = 0;
            continueIcon.SetActive(false);
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
            _canContinueToNextLine = false;
            continueIcon.SetActive(false);

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
                color.a = actorToDisplay.sprite ? 1 : 0;
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

            float messageProgress = _messageIndex / (float)_currentMessages.Length;
            float messageContribution = 1 / (float)_currentMessages.Length;

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

                _progressCallback?.Invoke(messageProgress + messageContribution *
                    (messageText.maxVisibleCharacters / (float)_currentMessages[_messageIndex].text.Length));
            }

            if (!IsEndOfConversation())
            {
                continueIcon.SetActive(true);
            }

            _canContinueToNextLine = true;
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

            _progressCallback?.Invoke(_messageIndex / (float)_currentMessages.Length);
        }

        private IEnumerator ExitDialogue()
        {
            yield return null;
            if (useUIViewSystem)
            {
                UIManager.Instance.Back();
            }

            DialogueIsPlaying = false;
            _listener.Event.Raise(false);

            _onEndCallback?.Invoke();
            _onEndCallback = null;
            _progressCallback = null;
        }

        private bool IsEndOfConversation()
        {
            return _messageIndex + 1 == _currentMessages.Length;
        }

        private void Update()
        {
            if (useUIViewSystem && UIManager.Instance.GetCurrentView() != dialogueView)
            {
                return;
            }

            if (_keyboardConfirmAction.WasPressedThisFrame() || _mouseConfirmAction.WasPressedThisFrame())
            {
                if (_canContinueToNextLine)
                {
                    confirmAudio.Play2D();
                    if (_messageIndex >= _currentMessages.Length)
                    {
                        return;
                    }

                    if (_currentMessages[_messageIndex].actor.voice != null)
                    {
                        _currentMessages[_messageIndex].actor.voice.StopAll();
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