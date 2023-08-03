using Dialogue;
using UnityEngine;
using Utils;

namespace Needs.Needs
{
    public class MusicNeed : MonoBehaviour, INeed
    {
        [MetricsRange(-1.0f, 1.0f)] [SerializeField] private NeedMetrics expiryPunishment = new NeedMetrics
        {
            hunger = 0.0f,
            thirst = 0.0f,
            enjoyment = -0.5f
        };
        [SerializeField] private float expirationTime = 20.0f;
        [SerializeField] private RandomConversationSO randomConversations;
        private float _currentTime;

        public void Awake()
        {
            ResetNeed();
        }

        public void UpdateTimer(float deltaTime)
        {
            _currentTime += deltaTime;
        }

        public void ResetNeed()
        {
            _currentTime = 0.0f;
        }

        public NeedType GetNeedType()
        {
            return NeedType.Music;
        }

        public NeedMetrics GetPunishment()
        {
            return expiryPunishment;
        }

        public bool IsExpired()
        {
            return _currentTime >= expirationTime;
        }

        public RandomConversationSO GetRandomConversations()
        {
            return randomConversations;
        }
    }
}