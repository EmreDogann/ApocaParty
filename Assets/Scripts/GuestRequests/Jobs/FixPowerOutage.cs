using Electricity;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class FixPowerOutage : Job
    {
        public float Duration = 1.0f;
        [SerializeField] private ElectricalBox _electricalBox;

        public override void Exit()
        {
            _electricalBox.PowerFixed();
        }

        public override float GetProgressPercentage()
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration()
        {
            return Duration;
        }
    }
}