using Audio;
using GuestRequests.Jobs;
using GuestRequests.Requests;
using Interactions.Interactables;
using PartyEvents;
using UnityEngine;

[RequireComponent(typeof(BuntingFallEvent), typeof(SpriteRenderer), typeof(BuntingRequest))]
[RequireComponent(typeof(RequestInteractable))]
public class Bunting : MonoBehaviour
{
    [SerializeField] private Sprite buntingFallenSprite;
    [SerializeField] private AudioSO fallAudio;
    [SerializeField] private AudioSO fixAudio;

    [Range(0.0f, 1.0f)] [SerializeField] private float buntingFallChance = 0.1f;
    [SerializeField] private float buntingFallCooldown = 10.0f;
    [SerializeField] private float buntingFallCheckFrequency = 0.5f;
    [SerializeField] private Vector2 buntingFallOffset;

    private float _currentTime;
    private bool _isBuntingFallen;
    private BuntingFallEvent _buntingFallEvent;
    private RequestInteractable _requestInteractable;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _buntingFallEvent = GetComponent<BuntingFallEvent>();
        _requestInteractable = GetComponent<RequestInteractable>();
        _requestInteractable.SetInteractableActive(false);
    }

    private void OnEnable()
    {
        FixBunting.BuntingFixed += BuntingFixed;
    }

    private void OnDisable()
    {
        FixBunting.BuntingFixed -= BuntingFixed;
    }

    private void Update()
    {
        if (_isBuntingFallen)
        {
            return;
        }

        _currentTime += Time.deltaTime;

        if (_currentTime < buntingFallCheckFrequency)
        {
            return;
        }

        _currentTime = 0.0f;
        if (Random.Range(0.0f, 1.0f) < buntingFallChance)
        {
            _isBuntingFallen = true;
            fallAudio.Play(transform.position);
            _spriteRenderer.sprite = buntingFallenSprite;
            transform.position += (Vector3)buntingFallOffset;

            _buntingFallEvent.TriggerEvent();
            _requestInteractable.SetInteractableActive(true);

            _currentTime = -buntingFallCooldown;
        }
    }

    private void BuntingFixed()
    {
        _requestInteractable.SetInteractableActive(false);
        fixAudio.Play(transform.position);
        _isBuntingFallen = false;
        transform.position -= (Vector3)buntingFallOffset;
    }
}