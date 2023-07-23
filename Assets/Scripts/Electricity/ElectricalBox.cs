using System;
using System.Collections.Generic;
using System.Linq;
using Electricity;
using Interactions.Interactables;
using PartyEvents;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PowerOutageEvent), typeof(RequestInteractable))]
public class ElectricalBox : MonoBehaviour
{
    private List<IElectricalAppliance> _appliances;
    [SerializeField] private float powerOutageCheckFrequency = 6.0f;
    [SerializeField] private float powerOutageCooldown = 10.0f;
    [SerializeField] private float powerOutageChance = 0.1f;

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
        _requestInteractable = GetComponent<RequestInteractable>();
        _requestInteractable.SetInteractableActive(false);

        _powerOutageEvent = GetComponent<PowerOutageEvent>();
        _appliances = FindObjectsOfType<MonoBehaviour>()
            .OfType<IElectricalAppliance>()
            .Where(a => ((MonoBehaviour)a).enabled)
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

            if (Random.Range(0.0f, 1.0f) < powerOutageChance)
            {
                _isPowerOn = false;
                _currentTime -= powerOutageCooldown;
                _requestInteractable.SetInteractableActive(true);

                OnPowerOutage?.Invoke();
                _powerOutageEvent.TriggerEvent();
            }
        }
    }

    public void PowerFixed()
    {
        _isPowerOn = true;
        _requestInteractable.SetInteractableActive(false);
        OnPowerFixed?.Invoke();
    }
}