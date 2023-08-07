using Dialogue;
using UnityEngine;
using Utils;

namespace Needs.Needs
{
    public class FoodNeed : MonoBehaviour, INeed
    {
        [MetricsRange(-1.0f, 1.0f)] [SerializeField] private NeedMetrics expiryPunishment = new NeedMetrics
        {
            hunger = -0.5f,
            thirst = 0.0f,
            enjoyment = -0.4f
        };
        [SerializeField] private float expirationTime = 20.0f;
        [SerializeField] private RandomConversationSO randomConversations;
        private float _currentTime;
        private bool _isPaused;

        public void Awake()
        {
            ResetNeed();
        }

        public void UpdateTimer(float deltaTime)
        {
            if (!_isPaused)
            {
                _currentTime += deltaTime;
            }
        }

        public void ResetNeed(float startingTime = 0.0f)
        {
            _currentTime = startingTime;
        }

        public NeedType GetNeedType()
        {
            return NeedType.Food;
        }

        public NeedMetrics GetPunishment()
        {
            return expiryPunishment;
        }

        public float GetTimerProgress()
        {
            return 1 - Mathf.Clamp01(_currentTime / expirationTime);
        }

        public void ExpireNeed()
        {
            _currentTime = expirationTime;
        }

        public void SetNeedPause(bool isPaused)
        {
            _isPaused = isPaused;
        }

        public bool IsExpired()
        {
            return _currentTime >= expirationTime;
        }

        public bool IsPaused()
        {
            return _isPaused;
        }

        public RandomConversationSO GetRandomConversations()
        {
            return randomConversations;
        }
    }
}