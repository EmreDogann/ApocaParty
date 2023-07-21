﻿using System;
using System.Collections.Generic;
using System.Linq;
using Electricity;
using PartyEvents;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PowerOutageEvent))]
public class ElectricalBox : MonoBehaviour
{
    private List<IElectricalAppliance> _appliances;
    [SerializeField] private float powerOutageCheckFrequency = 6.0f;
    [SerializeField] private float powerOutageChance = 0.1f;

    private PowerOutageEvent _powerOutageEvent;
    private float _currentTime;

    public static event Action OnPowerOutage;
    public static event Action OnPowerFixed;

    private void Awake()
    {
        _powerOutageEvent = GetComponent<PowerOutageEvent>();
        _appliances = FindObjectsOfType<MonoBehaviour>()
            .OfType<IElectricalAppliance>()
            .Where(a => ((MonoBehaviour)a).enabled)
            .ToList();
    }

    private void Update()
    {
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
                Debug.Log("Power Outage!");
                OnPowerOutage?.Invoke();
                _powerOutageEvent.TriggerEvent();
            }
        }
    }

    public void PowerFixed()
    {
        OnPowerFixed?.Invoke();
    }
}