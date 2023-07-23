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
            enjoyment = -0.2f,
            movement = 0.0f
        };
        [SerializeField] private float expirationTime = 20.0f;
        [SerializeField] private RandomConversationSO randomConversations;
        private float _startTime;

        public void Awake()
        {
            _startTime = Time.time;
        }

        public NeedType GetNeedType()
        {
            return NeedType.Drink;
        }

        public NeedMetrics GetPunishment()
        {
            return expiryPunishment;
        }

        public bool IsExpired()
        {
            return Time.time - _startTime >= expirationTime;
        }

        public RandomConversationSO GetRandomConversations()
        {
            return randomConversations;
        }
    }
}