using System;
using Audio;
using GuestRequests.Requests;
using MyBox;
using UnityEngine;

namespace Interactions.Interactables
{
    public class StoveInteractable : RequestInteractable
    {
        [Separator("Burning Audio")]
        [SerializeField] private AudioSO _burningStartAudio;
        [SerializeField] private AudioSO _burningAudio;

        [Separator("Particles")]
        [SerializeField] private ParticleSystem _fireParticleSystem;
        [SerializeField] private ParticleSystem _badHighlightParticleSystem;

        public static event Action OnFireExtinguished;

        private void Awake()
        {
            SetInteractableActive(false);
        }

        private void OnEnable()
        {
            FoodRequest.OnFire += OnFire;
            request.OnRequestCompleted += OnRequestCompleted;
        }

        private void OnDisable()
        {
            FoodRequest.OnFire -= OnFire;
            request.OnRequestCompleted -= OnRequestCompleted;
        }

        private void OnFire()
        {
            _badHighlightParticleSystem.Play();
            _fireParticleSystem.Play();

            _burningStartAudio.Stop();
            _burningAudio.Stop();

            _burningStartAudio.Play(transform.position);
            _burningAudio.Play(transform.position, true, 3.0f);

            SetInteractableActive(true);
        }

        private void OnRequestCompleted()
        {
            _badHighlightParticleSystem.Stop();
            _fireParticleSystem.Stop();
            _burningAudio.Stop(true, 5.0f);

            OnFireExtinguished?.Invoke();
            SetInteractableActive(false);
        }
    }
}