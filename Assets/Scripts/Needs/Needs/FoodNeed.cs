using UnityEngine;

namespace Needs.Needs
{
    public class FoodNeed : INeed
    {
        private readonly NeedType _needType = NeedType.Food;
        private readonly NeedMetrics _needReward;
        private readonly float _expirationTime = 3.0f;
        private readonly float _startTime;

        public FoodNeed()
        {
            _startTime = Time.time;
            _needReward = new NeedMetrics
            {
                hunger = 0.5f,
                thirst = 0.0f,
                enjoyment = 0.3f,
                movement = -0.2f
            };
        }

        public NeedType GetNeedType()
        {
            return _needType;
        }

        public NeedMetrics GetReward()
        {
            return _needReward;
        }

        public bool IsExpired()
        {
            return Time.time - _startTime >= _expirationTime;
        }
    }
}