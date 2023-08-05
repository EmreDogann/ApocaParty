using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Interactions.Interactables;
using PartyEvents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Electricity
{
    [RequireComponent(typeof(PowerOutageEvent), typeof(RequestInteractable))]
    public class ElectricalBox : MonoBehaviour
    {
        private List<IElectricalAppliance> _appliances;
        [Range(0.0f, 1.0f)] [SerializeField] private float powerOutageChance = 0.1f;
        [SerializeField] private float powerOutageCooldown = 10.0f;
        [SerializeField] private float powerOutageCheckFrequency = 6.0f;
        [SerializeField] private ParticleSystem badHighlight;

        [SerializeField] private AudioSO outageAudio;
        [SerializeField] private AudioSO fixAudio;

        private PowerOutageEvent _powerOutageEvent;
        private RequestInteractable _requestInteractable;

        private float _currentTime;

        public static event Action OnPowerOutage;
        public static event Action OnPowerFixed;

        public static bool IsPowerOn()
        {
            return _isPowerOn;
        }

        private static bool _isPowerOn = true;

        private void Awake()
        {
            _isPowerOn = true;

            _requestInteractable = GetComponent<RequestInteractable>();
            _requestInteractable.SetInteractableActive(false);

            _powerOutageEvent = GetComponent<PowerOutageEvent>();
            _appliances = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IElectricalAppliance>()
                .Where(a => ((MonoBehaviour)a).isActiveAndEnabled)
                .ToList();
        }

        private void Update()
        {
            if (!_isPowerOn)
            {
                return;
            }

            _currentTime += Time.deltaTime;
            if (_currentTime >= powerOutageCheckFrequency)
            {
                _currentTime = 0.0f;
                foreach (IElectricalAppliance appliance in _appliances)
                {
                    if (!appliance.IsInUse())
                    {
                        return;
                    }
                }

                if (Random.Range(0.0f, 1.0f) <= powerOutageChance)
                {
                    _isPowerOn = false;
                    _currentTime = -powerOutageCooldown;
                    _requestInteractable.SetInteractableActive(true);

                    outageAudio.Play(transform.position);
                    badHighlight.Play();

                    OnPowerOutage?.Invoke();
                    _powerOutageEvent.TriggerEvent();
                }
            }
        }

        public void PowerFixed()
        {
            _isPowerOn = true;

            fixAudio.Play2D();
            badHighlight.Stop();

            _requestInteractable.SetInteractableActive(false);
            OnPowerFixed?.Invoke();
        }
    }
}