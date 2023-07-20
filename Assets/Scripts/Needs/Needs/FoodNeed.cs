using UnityEngine;

namespace Needs.Needs
{
    public class FoodNeed : INeed
    {
        private readonly NeedType _needType = NeedType.Food;
        private float _needReward;
        private readonly float _expirationTime = 20.0f;
        private readonly float _startTime;

        public FoodNeed()
        {
            _startTime = Time.time;
        }

        public NeedType GetNeedType()
        {
            return _needType;
        }

        public float GetNeedReward()
        {
            return _needReward;
        }

        public bool IsExpired()
        {
            return Time.time - _startTime >= _expirationTime;
        }
    }
}