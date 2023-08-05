using Dialogue;
using UnityEngine;
using Utils;

namespace Needs.Needs
{
    public class DrinkNeed : MonoBehaviour, INeed
    {
        [MetricsRange(-1.0f, 1.0f)] [SerializeField] private NeedMetrics expiryPunishment = new NeedMetrics
        {
            hunger = 0.0f,
            thirst = -0.5f,
            enjoyment = -0.2f
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
            return NeedType.Drink;
        }

        public NeedMetrics GetPunishment()
        {
            return expiryPunishment;
        }

        public float GetTimerProgress()
        {
            return 1 - Mathf.Clamp01(_currentTime / expirationTime);
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