using System;
using Consumable;
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

namespace Player
{
    [RequireComponent(typeof(CharacterBlackboard), typeof(NavMeshAgent), typeof(DisplayAgentPath))]
    [RequireComponent(typeof(PlateMouseInteraction))]
    public class PlayerController : MonoBehaviour, IRequestOwner, IWaiter
    {
        [Separator("Movement")]
        [SerializeField] private InputActionReference moveButton;
        [SerializeField] private float distanceThreshold = 0.01f;
        [NavMeshSelector] [SerializeField] private int ignoreAreaCosts;
        [SerializeField] private Transform holderTransform;

        [Separator("Path Rendering")]
        [SerializeField] private DisplayAgentPath pathDisplayer;

        private NavMeshAgent _agent;
        private Camera _mainCamera;
        private CharacterBlackboard _blackboard;
        private PlateMouseInteraction _plateInteraction;

        private Request _currentRequest;
        private Transform _target;
        private IConsumable _holdingConsumable;
        private IConsumable _targetConsumable;
        private readonly int _waiterID = Guid.NewGuid().GetHashCode();

        private void Awake()
        {
            _blackboard = GetComponent<CharacterBlackboard>();
            _mainCamera = Camera.main;
            _agent = GetComponent<NavMeshAgent>();
            pathDisplayer = GetComponent<DisplayAgentPath>();
            _plateInteraction = GetComponent<PlateMouseInteraction>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.SetAreaCost(ignoreAreaCosts, 1.0f);
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
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            _blackboard.IsMoving = _agent.hasPath;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_targetConsumable != null)
            {
                if (_targetConsumable == other.GetComponent<IConsumable>())
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