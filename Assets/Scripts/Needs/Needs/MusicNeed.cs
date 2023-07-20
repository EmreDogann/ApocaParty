using UnityEngine;

namespace Needs.Needs
{
    public class MusicNeed : INeed
    {
        private readonly NeedMetrics _needPunishment;
        private readonly float _expirationTime = 20.0f;
        private readonly float _startTime;

        public MusicNeed()
        {
            _startTime = Time.time;
            _needPunishment = new NeedMetrics
            {
                hunger = 0.0f,
                thirst = 0.0f,
                enjoyment = -0.5f,
                movement = 0.1f
            };
        }

        public NeedType GetNeedType()
        {
            return NeedType.Music;
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