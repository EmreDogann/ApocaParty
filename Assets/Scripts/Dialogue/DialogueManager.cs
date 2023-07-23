using System.Collections;
using Actors;
using System.Collections.Generic;
using Events;
using MyBox;
using TMPro;
using UI;
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

        [SerializeField]
        // List of all actors, all SOs should be assigned as the names in those SOs used to parse Dialogue files
        public List<ActorSO> allActorSos;

        // public TextAsset tempScript;
        
        public static DialogueManager Instance;
        public bool DialogueIsPlaying { get; private set; }
        private bool _dialogueIsPaused;

        private BoolEventListener _listener;
        private Coroutine animationCoroutine;

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
            _listener.Event.Raise(true);
            DialogueIsPlaying = true;

            // Messages.ParseMessages(tempScript, allActorSos);
            
            
            UIManager.Instance.Show(dialogueView);
            animationCoroutine=StartCoroutine(DisplayMessage());
        }


        private IEnumerator DisplayMessage()
        {
            ActorSO actorToDisplay = _currentMessages[_messageIndex].actor;
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
            messageText.text=string.Empty;
            foreach(char c in _currentMessages[_messageIndex].message.ToCharArray()){
                messageText.text += c;
                Debug.Log(c);
                yield return new WaitForSecondsRealtime(0.1f);
            }


        }
        
        private void DisplayEntireMessage()
        {
            messageText.text = string.Empty;
            Message messageToDisplay = _currentMessages[_messageIndex];
            messageText.text = messageToDisplay.message;

            ActorSO actorToDisplay = messageToDisplay.actor;
            actorName.text = actorToDisplay.name;
            actorImage.sprite = actorToDisplay.sprite;
        }
        // private void DisplayMessage()
        // {
        //     Message messageToDisplay = _currentMessages[_messageIndex];
        //     messageText.text = messageToDisplay.message;

        //     ActorSO actorToDisplay = messageToDisplay.actor;
        //     actorName.text = actorToDisplay.name;
        //     actorImage.sprite = actorToDisplay.sprite;
        // }

        public void NextMessage()
        {
            _messageIndex++;
            if (_messageIndex < _currentMessages.Length)
            {
                animationCoroutine=StartCoroutine(DisplayMessage());
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
        }

        private void OnGamePaused(bool isPaused)
        {
            if (!DialogueIsPlaying)
            {
                return;
            }

            _dialogueIsPaused = isPaused;
        }

        private void Update()
        {
            if (confirmAction.action.WasPressedThisFrame() && UIManager.Instance.GetCurrentView() == dialogueView)
            {
                if(_messageIndex>=_currentMessages.Length){
                    return;
                }
                if(messageText.text!=_currentMessages[_messageIndex].message){
                    Debug.Log("entire");
                    if(animationCoroutine!=null){
                        StopCoroutine(animationCoroutine);
                    }
                    DisplayEntireMessage();
                }
                else{
                    Debug.Log("next");
                    NextMessage();
                }
                
            }
        }
    }
}