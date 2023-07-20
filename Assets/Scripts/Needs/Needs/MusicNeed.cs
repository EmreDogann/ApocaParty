using UnityEngine;

namespace Needs.Needs
{
    public class MusicNeed : INeed
    {
        private readonly NeedMetrics _needReward;
        private readonly float _expirationTime = 20.0f;
        private readonly float _startTime;

        public MusicNeed()
        {
            _startTime = Time.time;
            _needReward = new NeedMetrics
            {
                hunger = 0.0f,
                thirst = 0.0f,
                enjoyment = 0.5f,
                movement = -0.1f
            };
        }

        public NeedType GetNeedType()
        {
            return NeedType.Music;
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