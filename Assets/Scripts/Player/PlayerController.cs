using System;
using System.Collections;
using Audio;
using Consumable;
using DG.Tweening;
using Electricity;
using GuestRequests;
using GuestRequests.Requests;
using Interactions;
using Interactions.Interactables;
using MyBox;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Utils;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent), typeof(DisplayAgentPath), typeof(PlateMouseInteraction))]
    public class PlayerController : MonoBehaviour, IRequestOwner, IWaiter
    {
        [Separator("Movement")]
        [SerializeField] private InputActionReference moveButton;
        [SerializeField] private float distanceThreshold = 0.01f;
        [NavMeshSelector] [SerializeField] private int ignoreAreaCosts;
        [SerializeField] private Transform holderTransform;

        [Separator("Path Rendering")]
        [SerializeField] private DisplayAgentPath pathDisplayer;

        [Separator("Other Settings")]
        [Range(0.0f, 1.0f)] public float chanceToSpillFood;
        [Range(0.0f, 1.0f)] public float chanceToSlip;
        public float spillFoodCheckFrequency;
        [SerializeField] private AudioSO slipAudio;
        [SerializeField] private float cleanupTime;

        private float _spillFoodTimer;
        private bool _isSlipping;
        private bool _isCleaningUp;

        private NavMeshAgent _agent;
        private Camera _mainCamera;
        private PlateMouseInteraction _plateInteraction;

        private Request _currentRequest;
        private Transform _target;
        private IConsumable _holdingConsumable;
        private IConsumable _targetConsumable;
        private readonly int _waiterID = Guid.NewGuid().GetHashCode();

        private int _spillLayer;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();
            pathDisplayer = GetComponent<DisplayAgentPath>();
            _plateInteraction = GetComponent<PlateMouseInteraction>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.SetAreaCost(ignoreAreaCosts, 1.0f);

            _spillLayer = LayerMask.NameToLayer("Drink");
        }

        private void OnEnable()
        {
            MouseInteraction.OnInteract += OnInteraction;
        }

        private void OnDisable()
        {
            MouseInteraction.OnInteract -= OnInteraction;
        }

        private void OnInteraction(InteractableBase interactable)
        {
            if (_currentRequest != null || _holdingConsumable != null)
            {
                return;
            }

            switch (interactable)
            {
                case IInteractableRequest interactableRequest:
                    Request request = interactableRequest.GetRequest();
                    switch (request)
                    {
                        case PowerRequest _:
                            if (ElectricalBox.IsPowerOn())
                            {
                                return;
                            }

                            break;
                        case FoodRequest _:
                            if (request.IsRequestFailed())
                            {
                                SetDestinationAndDisplayPath(request.transform.position);
                            }
                            else if (_holdingConsumable == null && request.IsRequestCompleted())
                            {
                                _targetConsumable = request as IConsumable;
                                _targetConsumable.Claim();
                                _target = null;
                            }

                            return;
                        case DrinkRefillRequest _:
                            if (DrinksTable.Instance.IsDrinksTableFull())
                            {
                                return;
                            }

                            break;
                    }

                    if (request.IsRequestStarted() || !request.TryStartRequest() || request.GetRequestOwner() != null)
                    {
                        return;
                    }

                    SetDestinationAndDisplayPath(request.GetStartingPosition());
                    _currentRequest = request;
                    _currentRequest.AssignOwner(this);

                    _currentRequest.OnRequestCompleted += OnRequestCompleted;

                    break;
                case GuestInteractable guestInteractable:
                    _target = guestInteractable.WaiterTarget.GetDestinationTransform();
                    guestInteractable.WaiterTarget.GiveWaiterID(_waiterID);
                    break;
                case FridgeInteractable fridgeInteractable:
                    FoodRequest foodRequest = fridgeInteractable.Fridge.TryGetFood();
                    if (foodRequest == null)
                    {
                        // TODO: Play error sound.
                        return;
                    }

                    SetDestinationAndDisplayPath(foodRequest.GetStartingPosition());
                    _currentRequest = foodRequest;
                    _currentRequest.OnRequestCompleted += OnRequestCompleted;

                    foodRequest.AssignOwner(this);
                    break;
                case SpillInteractable spillInteractable:
                    _target = spillInteractable.transform;
                    _targetConsumable = spillInteractable.Consumable;
                    break;
                case null:
                    break;
            }
        }

        private void OnRequestCompleted()
        {
            _currentRequest.OnRequestCompleted -= OnRequestCompleted;
            _currentRequest = null;
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f || _isSlipping || _isCleaningUp)
            {
                return;
            }

            if (_holdingConsumable != null)
            {
                _holdingConsumable.GetTransform().position = holderTransform.position;
                if (_target == null)
                {
                    switch (_plateInteraction.CheckForPlateInteraction())
                    {
                        case PlateInteractable plateInteractable:
                            _target = plateInteractable.WaiterTarget.GetDestinationTransform();
                            plateInteractable.WaiterTarget.GiveWaiterID(_waiterID);

                            break;
                    }
                }
            }

            if (_target != null)
            {
                SetDestinationAndDisplayPath(_target.position);
            }

            if (_currentRequest && _currentRequest.IsRequestStarted())
            {
                _currentRequest.UpdateRequest(Time.deltaTime * 2.0f);
                return;
            }

            if (moveButton.action.WasPressedThisFrame() && !_currentRequest)
            {
                Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
                mouseWorldPosition.z = 0;
                SetDestinationAndDisplayPath(mouseWorldPosition);
            }

            if (Vector3.SqrMagnitude(transform.position - _agent.destination) < distanceThreshold * distanceThreshold)
            {
                if (_currentRequest != null)
                {
                    _currentRequest.ActivateRequest();
                }

                pathDisplayer.HidePath();
            }
        }

        private void Slip()
        {
            _isSlipping = true;
            _agent.ResetPath();
            pathDisplayer.HidePath();

            transform.DOShakeRotation(1.5f, new Vector3(0.0f, 0.0f, 40.0f), 5, 1, true, ShakeRandomnessMode.Harmonic)
                .OnComplete(
                    () => { _isSlipping = false; });
            slipAudio.Play(transform.position);
        }

        private IEnumerator CleanupSpill(SpillInteractable spillInteractable)
        {
            _isCleaningUp = true;
            _agent.ResetPath();
            pathDisplayer.HidePath();

            yield return new WaitForSeconds(cleanupTime);

            spillInteractable.Consumable.Cleanup();
            _isCleaningUp = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_targetConsumable != null)
            {
                IConsumable consumable = other.GetComponent<IConsumable>();
                if (_targetConsumable == consumable && !consumable.IsSpilled())
                {
                    _holdingConsumable = _targetConsumable;
                    _holdingConsumable.Claim();
                    _targetConsumable = null;
                }
            }

            if (_target != null)
            {
                IWaiterTarget waiterTarget = other.GetComponent<IWaiterTarget>();
                if (waiterTarget != null && waiterTarget.GetWaiterID() == _waiterID)
                {
                    waiterTarget.WaiterInteracted(this);
                    _target = null;
                    _holdingConsumable = null;
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isCleaningUp || _isSlipping || _agent.hasPath)
            {
                return;
            }

            if (_target != null)
            {
                IWaiterTarget waiterTarget = other.GetComponent<IWaiterTarget>();
                if (waiterTarget != null && waiterTarget.GetWaiterID() == _waiterID)
                {
                    waiterTarget.WaiterInteracted(this);
                    _target = null;
                    _holdingConsumable = null;
                }

                SpillInteractable spillInteractable = other.GetComponent<SpillInteractable>();
                if (spillInteractable != null && ReferenceEquals(_targetConsumable, spillInteractable.Consumable))
                {
                    StartCoroutine(CleanupSpill(spillInteractable));
                    _target = null;
                    _holdingConsumable = null;
                    _targetConsumable = null;
                    return;
                }
            }

            if (other.gameObject.layer != _spillLayer)
            {
                return;
            }

            _spillFoodTimer += Time.deltaTime;
            if (_spillFoodTimer > spillFoodCheckFrequency)
            {
                _spillFoodTimer = 0.0f;
                if (_holdingConsumable != null && Random.Range(0.0f, 1.0f) < chanceToSpillFood)
                {
                    _holdingConsumable.Spill();
                    _holdingConsumable = null;
                    Slip();
                }
                else if (Random.Range(0.0f, 1.0f) < chanceToSlip)
                {
                    Slip();
                }
            }
        }

        public void SetDestination(Vector3 target)
        {
            _agent.SetDestination(target);
        }

        public void SetDestinationAndDisplayPath(Vector3 target)
        {
            _agent.SetDestination(target);
            pathDisplayer.DisplayPath();
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        public Transform GetHoldingTransform()
        {
            return holderTransform;
        }

        public void OwnerRemoved()
        {
            _currentRequest = null;
        }

        public IConsumable GetConsumable()
        {
            return _holdingConsumable;
        }

        public int GetWaiterID()
        {
            return _waiterID;
        }
    }
}