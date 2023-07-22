﻿using UnityEngine;

namespace Electricity
{
    public class Stove : MonoBehaviour, IElectricalAppliance
    {
        private bool _isBeingUsed;

        public bool IsInUse()
        {
            return _isBeingUsed;
        }

        public void TurnOn()
        {
            _isBeingUsed = true;
        }

        public void TurnOff()
        {
            _isBeingUsed = false;
        }
    }
}