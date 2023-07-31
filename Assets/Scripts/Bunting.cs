using Audio;
using GuestRequests.Jobs;
using GuestRequests.Requests;
using Interactions.Interactables;
using PartyEvents;
using UnityEngine;

[RequireComponent(typeof(BuntingFallEvent), typeof(BuntingRequest), typeof(RequestInteractable))]
public class Bunting : MonoBehaviour
{
    [SerializeField] private bool enableBuntingFalling;

    [SerializeField] private SpriteRenderer buntingUpSprite;
    [SerializeField] private SpriteRenderer buntingDownSprite;
    [SerializeField] private AudioSO fallAudio;
    [SerializeField] private AudioSO fixAudio;

    [Range(0.0f, 1.0f)] [SerializeField] private float buntingFallChance = 0.1f;
    [SerializeField] private float buntingFallCooldown = 10.0f;
    [SerializeField] private float buntingFallCheckFrequency = 0.5f;

    private float _currentTime;
    private bool _isBuntingFallen;
    private BuntingFallEvent _buntingFallEvent;
    private RequestInteractable _requestInteractable;

    private void Awake()
    {
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

    public void SetBuntingFallActive(bool isActive)
    {
        enableBuntingFalling = isActive;
    }

    public void TriggerBuntingFall()
    {
        _isBuntingFallen = true;
        fallAudio.Play(transform.position);

        buntingUpSprite.transform.gameObject.SetActive(false);
        buntingDownSprite.transform.gameObject.SetActive(true);

        _buntingFallEvent.TriggerEvent();
        _requestInteractable.SetInteractableActive(true);
    }

    private void Update()
    {
        if (!enableBuntingFalling || _isBuntingFallen)
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
            TriggerBuntingFall();
            _currentTime = -buntingFallCooldown;
        }
    }

    private void BuntingFixed()
    {
        _requestInteractable.SetInteractableActive(false);
        fixAudio.Play(transform.position);
        _isBuntingFallen = false;

        buntingUpSprite.transform.gameObject.SetActive(true);
        buntingDownSprite.transform.gameObject.SetActive(false);
    }
}