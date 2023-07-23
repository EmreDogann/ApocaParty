using Consumable;
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
        private GuestInteractable _targetGuest;
        private IConsumable _holdingConsumable;
        private int waiterID;

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
                            break;
                        case FoodRequest _:
                            if (interactableRequest.GetRequest().IsRequestFailed())
                            {
                                SetDestinationAndDisplayPath(interactableRequest.GetRequest().transform.position);
                            }
                            else if (interactableRequest.GetRequest().IsRequestCompleted())
                            {
                                _holdingConsumable = interactableRequest as IConsumable;
                                _holdingConsumable?.Claim();

                                SetDestinationAndDisplayPath(interactableRequest.GetRequest().transform.position);
                            }

                            break;
                    }

                    break;
                case GuestInteractable guestInteractable:
                    _targetGuest = guestInteractable;
                    waiterID = guestInteractable.PlayerInteracted();
                    SetDestinationAndDisplayPath(guestInteractable.transform.position);
                    break;
                case PlateInteractable plateInteractable:
                    if (_holdingConsumable != null)
                    {
                        waiterID = plateInteractable.AnnounceDelivery();
                        SetDestinationAndDisplayPath(plateInteractable.transform.position);
                    }

                    break;
            }
        }

        private void Update()
        {
            if (Time.timeScale == 0.0f)
            {
                return;
            }

            _blackboard.IsMoving = _agent.hasPath;

            if (_targetGuest != null)
            {
                SetDestinationAndDisplayPath(_targetGuest.transform.position);
            }

            if (_holdingConsumable != null)
            {
                _holdingConsumable.GetTransform().position = holderTransform.position;
                switch (plateInteraction.CheckForPlateInteraction())
                {
                    case PlateInteractable plateInteractable:
                        waiterID = plateInteractable.AnnounceDelivery();
                        SetDestinationAndDisplayPath(plateInteractable.transform.position);

                        break;
                }
            }

            if (_currentRequest)
            {
                _currentRequest.UpdateRequest(Time.deltaTime);
                if (_currentRequest.IsRequestCompleted())
                {
                    _currentRequest = null;
                }
                else
                {
                    return;
                }
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

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (_targetGuest != null && other.transform.CompareTag("Guest"))
        //     {
        //         GuestInteractable interactable = other.transform.GetComponent<GuestInteractable>();
        //         if (interactable == _targetGuest)
        //         {
        //             _targetGuest = null;
        //             _agent.ResetPath();
        //         }
        //     }
        // }

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

        public IConsumable GetFood()
        {
            waiterID = 0;
            _agent.ResetPath();
            return _holdingConsumable;
        }

        public void FinishInteraction()
        {
            waiterID = 0;
            _targetGuest = null;
            _agent.ResetPath();
        }

        public int GetWaiterID()
        {
            return waiterID;
        }
    }
}