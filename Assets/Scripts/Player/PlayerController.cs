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
        private PlateMouseInteraction plateInteraction;

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
            plateInteraction = GetComponent<PlateMouseInteraction>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.SetAreaCost(ignoreAreaCosts, 1.0f);
        }

        private void OnEnable()
        {
            InteractionSystem.OnInteract += OnInteraction;
        }

        private void OnDisable()
        {
            InteractionSystem.OnInteract -= OnInteraction;
        }

        private void OnInteraction(InteractableBase interactable)
        {
            switch (interactable)
            {
                case IInteractableRequest interactableRequest:
                    switch (interactableRequest.GetRequest())
                    {
                        case PowerRequest _:
                            if (ElectricalBox.IsPowerOn())
                            {
                                return;
                            }

                            if (!interactableRequest.GetRequest().TryStartRequest())
                            {
                                return;
                            }


                            _currentRequest = interactableRequest.GetRequest();
                            _currentRequest.AssignOwner(this);
                            _currentRequest.ActivateRequest();

                            _currentRequest.OnRequestCompleted += OnRequestCompleted;
                            break;
                        case FoodRequest _:
                            if (interactableRequest.GetRequest().IsRequestFailed())
                            {
                                SetDestinationAndDisplayPath(interactableRequest.GetRequest().transform.position);
                            }
                            else if (_holdingConsumable == null &&
                                     interactableRequest.GetRequest().IsRequestCompleted())
                            {
                                _targetConsumable = interactableRequest.GetRequest() as IConsumable;
                                SetDestinationAndDisplayPath(interactableRequest.GetRequest().transform.position);
                                _target = null;
                            }

                            break;
                    }

                    break;
                case GuestInteractable guestInteractable:
                    _target = guestInteractable.WaiterTarget.GetDestinationTransform();
                    guestInteractable.WaiterTarget.GiveWaiterID(_waiterID);
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
                    switch (plateInteraction.CheckForPlateInteraction())
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

            if (_currentRequest)
            {
                _currentRequest.UpdateRequest(Time.deltaTime);
                return;
            }

            if (moveButton.action.WasPressedThisFrame())
            {
                Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
                mouseWorldPosition.z = 0;
                SetDestinationAndDisplayPath(mouseWorldPosition);
            }

            if (Vector3.SqrMagnitude(transform.position - _agent.destination) < distanceThreshold * distanceThreshold)
            {
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

        public Transform GetHoldingPosition()
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