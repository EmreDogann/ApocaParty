using UnityEngine;
using UnityEngine.Events;

namespace Electricity
{
    public class PowerOutageListener : MonoBehaviour
    {
        public UnityEvent onPowerOutage;
        public UnityEvent onPowerFixed;

        private void OnEnable()
        {
            ElectricalBox.OnPowerOutage += OnPowerOutage;
            ElectricalBox.OnPowerFixed += OnPowerFixed;
        }

        private void OnDisable()
        {
            ElectricalBox.OnPowerOutage -= OnPowerOutage;
            ElectricalBox.OnPowerFixed -= OnPowerFixed;
        }

        private void OnPowerOutage()
        {
            onPowerOutage?.Invoke();
        }

        private void OnPowerFixed()
        {
            onPowerFixed?.Invoke();
        }
    }
}