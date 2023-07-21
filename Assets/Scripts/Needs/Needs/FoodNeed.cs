using UnityEngine;

namespace Needs.Needs
{
    public class FoodNeed : INeed
    {
        private readonly NeedMetrics _needPunishment;
        private readonly float _expirationTime = 20.0f;
        private readonly float _startTime;

        public FoodNeed()
        {
            _startTime = Time.time;
            _needPunishment = new NeedMetrics
            {
                hunger = -0.5f,
                thirst = 0.0f,
                enjoyment = -0.4f,
                movement = 0.2f
            };
        }

        public NeedType GetNeedType()
        {
            return NeedType.Food;
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