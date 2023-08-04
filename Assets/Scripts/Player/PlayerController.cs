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
using Utils;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(NavMeshAgent), typeof(DisplayAgentPath))]
    public class PlayerController : MonoBehaviour, IRequestOwner, IWaiter
    {
        [Separator("General")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private AudioSO errorSound;

        [Separator("Movement")]
        [SerializeField] private float distanceThreshold = 0.01f;
        [NavMeshSelector] [SerializeField] private int ignoreAreaCosts;
        [SerializeField] private Transform holderTransform;

        [Separator("Path Rendering")]
        [SerializeField] private DisplayAgentPath pathDisplayer;

        [Separator("UI")]
        [SerializeField] private ProgressBar progressBar;

        [Separator("Spill Settings")]
        [Range(0.0f, 1.0f)] public float chanceToSpillFood;
        [Range(0.0f, 1.0f)] public float chanceToSlip;
        public float spillFoodCheckFrequency;
        [SerializeField] private AudioSO slipAudio;
        [SerializeField] private float cleanupTime;

        private float _spillFoodTimer;
        private bool _isSlipping;
        private bool _isCleaningUp;

        private NavMeshAgent _agent;

        private Request _currentRequest;
        private Transform _targetDestination;
        private IWaiterTarget _waiterTarget;
        private IConsumable _holdingConsumable;
        private IConsumable _targetConsumable;
        private readonly int _waiterID = Guid.NewGuid().GetHashCode();

        private int _spillLayer;
        private int _characterLayer;

        private bool _isPlayerInputActive = true;
        internal float SearchRadius = 1.0f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            pathDisplayer = GetComponent<DisplayAgentPath>();

            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
            _agent.SetAreaCost(ignoreAreaCosts, 1.0f);

            _characterLayer = LayerMask.NameToLayer("Character");
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
            if (_currentRequest != null || _targetConsumable != null || _isSlipping || _isCleaningUp)
            {
                return;
            }

            Request request;
            switch (interactable)
            {
                case IInteractableRequest interactableRequest:
                    request = interactableRequest.GetRequest();
                    switch (request)
                    {
                        case PowerRequest _:
                            if (ElectricalBox.IsPowerOn())
                            {
                                return;
                            }

                            break;
                        case FoodRequest foodRequest:
                            if (foodRequest.IsRequestFailed())
                            {
                                SetDestinationAndDisplayPath(foodRequest.transform.position);
                            }
                            else if (_holdingConsumable == null && foodRequest.IsRequestCompleted())
                            {
                                _targetConsumable = foodRequest;
                                _targetConsumable.Claim();
                                if (_targetConsumable != null)
                                {
                                    SetDestinationAndDisplayPath(_targetConsumable.GetTransform().position);
                                }

                                _targetDestination = null;
                            }

                            return;
                    }

                    if (request.IsRequestStarted() || !request.TryStartRequest() || request.GetRequestOwner() != null)
                    {
                        return;
                    }

                    if (interactableRequest is StoveInteractable && _currentRequest != null)
                    {
                        _currentRequest.ResetRequest();
                        OwnerRemoved();
                        OnRequestCompleted();
                    }

                    SetDestinationAndDisplayPath(request.GetStartingPosition());
                    _currentRequest = request;
                    _currentRequest.AssignOwner(this);

                    _currentRequest.OnRequestCompleted += OnRequestCompleted;

                    break;
                case GuestInteractable guestInteractable:
                    if (guestInteractable.WaiterTarget.HasUnknownRequest() ||
                        !guestInteractable.WaiterTarget.HasConsumable() && _holdingConsumable != null)
                    {
                        _waiterTarget = guestInteractable.WaiterTarget;
                        _targetDestination = _waiterTarget.GetDestinationTransform();

                        _waiterTarget.GiveWaiterID(_waiterID);
                    }
                    else
                    {
                        errorSound.Play2D();
                    }

                    break;
                case FoodPileInteractable foodPileInteractable:
                    request = foodPileInteractable.FoodPile.TryGetFood();
                    if (request == null)
                    {
                        errorSound.Play2D();
                        return;
                    }

                    SetDestinationAndDisplayPath(request.GetStartingPosition());
                    _currentRequest = request;
                    request.AssignOwner(this);

                    _currentRequest.OnRequestCompleted += OnRequestCompleted;
                    break;
                case DrinksTableInteractable drinksTableInteractable:
                    if (drinksTableInteractable.IsDrinkAvailable())
                    {
                        if (_holdingConsumable == null)
                        {
                            _targetConsumable = drinksTableInteractable.TryGetDrink();
                            if (_targetConsumable != null)
                            {
                                SetDestinationAndDisplayPath(_targetConsumable.GetTransform().position);
                            }

                            _targetDestination = null;
                        }
                    }
                    else
                    {
                        request = drinksTableInteractable.TryRefill();
                        if (request == null)
                        {
                            return;
                        }

                        SetDestinationAndDisplayPath(request.GetStartingPosition());
                        _currentRequest = request;
                        _currentRequest.AssignOwner(this);

                        _currentRequest.OnRequestCompleted += OnRequestCompleted;
                    }

                    break;
                case SpillInteractable spillInteractable:
                    _targetDestination = spillInteractable.transform;
                    _targetConsumable = spillInteractable.Consumable;
                    _targetConsumable.StartCleanup();
                    break;
                case null:
                    break;
            }
        }

        private void OnRequestCompleted()
        {
            _currentRequest.OnRequestCompleted -= OnRequestCompleted;
            _currentRequest = null;

            progressBar.SetProgressBarActive(false);
            _agent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius, _agent.areaMask));
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
            }

            if (_targetDestination != null)
            {
                SetDestinationAndDisplayPath(_targetDestination.position);
            }

            if (_currentRequest && _currentRequest.IsRequestStarted())
            {
                _currentRequest.UpdateRequest(Time.deltaTime * 2.0f);
                if (_currentRequest)
                {
                    progressBar.SetProgressBarPercentage(_currentRequest.GetProgress());
                }

                return;
            }

            // if (InputManager.Instance.InteractPressed && !_currentRequest && _isPlayerInputActive)
            // {
            //     Vector3 mouseWorldPosition = _mainCamera.ScreenToWorldPoint(Mouse.current.position.value);
            //     mouseWorldPosition.z = 0;
            //     SetDestinationAndDisplayPath(mouseWorldPosition);
            // }

            if (Vector3.SqrMagnitude(transform.position - _agent.destination) < distanceThreshold * distanceThreshold)
            {
                _targetDestination = null;
                pathDisplayer.HidePath();

                if (_currentRequest != null)
                {
                    _currentRequest.ActivateRequest();
                    progressBar.SetProgressBarActive(true);
                    return;
                }

                if (_targetConsumable != null && !_targetConsumable.IsSpilled())
                {
                    if (!_targetConsumable.IsAvailable())
                    {
                        if (_targetConsumable is Drink && DrinksTable.Instance.IsDrinkAvailable())
                        {
                            _targetConsumable = DrinksTable.Instance.TryGetDrink();
                            if (_targetConsumable != null)
                            {
                                SetDestination(_targetConsumable.GetTransform().position);
                            }
                        }
                        else
                        {
                            _targetConsumable = null;
                            return;
                        }
                    }

                    _holdingConsumable = _targetConsumable;
                    _holdingConsumable.Claim();

                    _holdingConsumable.SetSorting(spriteRenderer.sortingLayerID, spriteRenderer.sortingOrder + 1);
                    _holdingConsumable.GetTransform().position = holderTransform.position;

                    _agent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius, _agent.areaMask));

                    _targetConsumable = null;
                    return;
                }

                if (_waiterTarget != null && _waiterTarget.IsAssignedWaiter() &&
                    _waiterTarget.GetWaiterID() == _waiterID)
                {
                    _waiterTarget.WaiterInteracted(this);

                    if (!_waiterTarget.HasUnknownRequest())
                    {
                        _holdingConsumable = null;
                    }

                    _agent.SetDestination(RandomNavmeshLocation(transform.position, SearchRadius, _agent.areaMask));

                    _waiterTarget = null;
                }
            }
        }

        internal Vector3 RandomNavmeshLocation(Vector3 position, float radius, int areaMask)
        {
            Vector3 finalPosition = position;
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = ClampMagnitude(Random.insideUnitCircle * radius, Mathf.Infinity, 2.0f);
                randomDirection += position;

                if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit, areaMask))
                {
                    if (Physics2D.OverlapCircle(raycastHit.position, 1.0f, 1 << _characterLayer) == null)
                    {
                        finalPosition = raycastHit.position;
                        break;
                    }
                }
            }

            finalPosition.z = 0;
            return finalPosition;
        }

        private Vector3 ClampMagnitude(Vector3 v, float max, float min)
        {
            double sm = v.sqrMagnitude;
            if (sm > max * (double)max)
            {
                return v.normalized * max;
            }

            if (sm < min * (double)min)
            {
                return v.normalized * min;
            }

            return v;
        }

        private void Slip()
        {
            _isSlipping = true;
            _targetDestination = null;
            _agent.ResetPath();
            pathDisplayer.HidePath();

            _waiterTarget?.WaiterCancelled();
            _waiterTarget = null;

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
            progressBar.SetProgressBarActive(true);

            float currentTime = 0.0f;
            while (currentTime < cleanupTime)
            {
                currentTime += Time.deltaTime;
                progressBar.SetProgressBarPercentage(currentTime / cleanupTime);
                yield return null;
            }

            spillInteractable.Consumable.Cleanup();
            progressBar.SetProgressBarActive(false);

            _isCleaningUp = false;
        }

        // private void OnTriggerEnter2D(Collider2D other)
        // {
        //     if (_target == null)
        //     {
        //         IWaiterTarget waiterTarget = other.GetComponent<IWaiterTarget>();
        //         if (waiterTarget != null && waiterTarget.IsAssignedWaiter() && waiterTarget.GetWaiterID() == _waiterID)
        //         {
        //             waiterTarget.WaiterInteracted(this);
        //             _target = null;
        //             _holdingConsumable = null;
        //         }
        //     }
        // }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (_isCleaningUp || _isSlipping || !_agent.hasPath)
            {
                return;
            }

            SpillInteractable spillInteractable = other.GetComponent<SpillInteractable>();
            if (spillInteractable != null && ReferenceEquals(_targetConsumable, spillInteractable.Consumable))
            {
                StartCoroutine(CleanupSpill(spillInteractable));
                _targetDestination = null;
                _holdingConsumable = null;
                _targetConsumable = null;
                return;
            }

            if (other.gameObject.layer != _spillLayer ||
                _targetConsumable != null && _targetConsumable.IsSpilled() ||
                _holdingConsumable != null && other.gameObject == _holdingConsumable.GetTransform().gameObject)
            {
                return;
            }

            _spillFoodTimer += Time.deltaTime;
            if (_spillFoodTimer < spillFoodCheckFrequency)
            {
                return;
            }

            _spillFoodTimer = 0.0f;
            if (_holdingConsumable != null)
            {
                if (Random.Range(0.0f, 1.0f) < chanceToSpillFood)
                {
                    _holdingConsumable.Spill();
                    _holdingConsumable = null;
                    Slip();
                }
            }
            else if (Random.Range(0.0f, 1.0f) < chanceToSlip)
            {
                Slip();
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
            _targetDestination = null;
            pathDisplayer.HidePath();
            _agent.ResetPath();
            progressBar.SetProgressBarActive(false);
        }

        public OwnerType GetOwnerType()
        {
            return OwnerType.Player;
        }

        public IConsumable GetConsumable()
        {
            return _holdingConsumable;
        }

        public int GetWaiterID()
        {
            return _waiterID;
        }

        public void SetPlayerInputActive(bool playerInput)
        {
            _isPlayerInputActive = playerInput;
        }
    }
}