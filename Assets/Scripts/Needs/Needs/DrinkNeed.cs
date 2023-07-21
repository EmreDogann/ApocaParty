using UnityEngine;

namespace Needs.Needs
{
    public class DrinkNeed : INeed
    {
        private readonly NeedMetrics _needPunishment;
        private readonly float _expirationTime = 20.0f;
        private readonly float _startTime;

        public DrinkNeed()
        {
            _startTime = Time.time;
            _needPunishment = new NeedMetrics
            {
                hunger = 0.0f,
                thirst = -0.5f,
                enjoyment = -0.2f,
                movement = 0.0f
            };
        }

        public NeedType GetNeedType()
        {
            return NeedType.Drink;
        }

        public NeedMetrics GetPunishment()
        {
            return _needPunishment;
        }

        public bool IsExpired()
        {
            return Time.time - _startTime >= _expirationTime;
        }
    }
}