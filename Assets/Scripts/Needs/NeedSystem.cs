using System;
using System.Collections.Generic;
using System.Linq;
using Dialogue;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using Utils;

namespace Needs
{
    [Serializable]
    public class NeedMetrics
    {
        public float hunger;
        public float thirst;
        public float enjoyment;

        public static NeedMetrics operator +(NeedMetrics a, NeedMetrics b)
        {
            a.hunger = Mathf.Clamp(a.hunger + b.hunger, 0.0f, 1.0f);
            a.thirst = Mathf.Clamp(a.thirst + b.thirst, 0.0f, 1.0f);
            a.enjoyment = Mathf.Clamp(a.enjoyment + b.enjoyment, 0.0f, 1.0f);

            return a;
        }

        public static NeedMetrics operator -(NeedMetrics a, NeedMetrics b)
        {
            a.hunger = Mathf.Clamp(a.hunger - b.hunger, 0.0f, 1.0f);
            a.thirst = Mathf.Clamp(a.thirst - b.thirst, 0.0f, 1.0f);
            a.enjoyment = Mathf.Clamp(a.enjoyment - b.enjoyment, 0.0f, 1.0f);

            return a;
        }

        public static NeedMetrics operator *(NeedMetrics a, int value)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = a.hunger * value,
                thirst = a.thirst * value,
                enjoyment = a.enjoyment * value
            };

            return metric;
        }

        public static NeedMetrics operator *(NeedMetrics a, float value)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = a.hunger * value,
                thirst = a.thirst * value,
                enjoyment = a.enjoyment * value
            };

            return metric;
        }

        public void SetAll(float value)
        {
            value = Mathf.Clamp(value, 0.0f, 1.0f);

            hunger = value;
            thirst = value;
            enjoyment = value;
        }
    }

    public class NeedSystem : MonoBehaviour
    {
        [SerializeField] private NeedsDisplayer needsDisplayer;
        [SerializeField] private float needCooldown = 10.0f;

        [SerializeField] private bool enableMoods;

        [ConditionalField(nameof(enableMoods))] [SerializeField] private Mood mood;

        [Tooltip("The point at which the guest will generate a need to satisfy the respective metric.")]
        [MetricsRange(0.0f, 1.0f)] [SerializeField] private NeedMetrics _metricsThreshold;

        [Tooltip("The rate at which the needs metrics tick down every second.")]
        [MetricsRange(0.0f, 0.1f)] [SerializeField] private NeedMetrics _metricsDepletionRate;

        [MetricsRange(0.0f, 1.0f)] [SerializeField] private NeedMetrics _currentMetrics;
        private List<INeed> _availableNeeds;
        private List<INeed> _currentNeeds;

        private readonly float _needCheckFrequency = 3.0f;
        private float _currentTime;
        private float _needTimer;

        public event Action OnNeedsResolved;
        public event Action<INeed> OnNewNeed;
        public event Action<NeedType> OnNeedFulfilled;

        private void Awake()
        {
            _currentNeeds = new List<INeed>();
            _availableNeeds = GetComponentsInChildren<INeed>().ToList();

            _currentTime = 0.0f;
            _needTimer = 0.0f;
        }

        public void Tick()
        {
            _currentMetrics -= _metricsDepletionRate * Time.deltaTime;

            if (enableMoods)
            {
                mood.Tick();
            }

            for (int i = _currentNeeds.Count - 1; i >= 0; i--)
            {
                _currentNeeds[i].UpdateTimer(Time.deltaTime);
                needsDisplayer.UpdateProgress(_currentNeeds[i].GetNeedType(), _currentNeeds[i].GetTimerProgress());
                if (_currentNeeds[i].IsExpired())
                {
                    if (enableMoods)
                    {
                        mood.ChangeMood(-1);
                    }

                    _currentMetrics += _currentNeeds[i].GetPunishment();
                    RemoveNeed(_currentNeeds[i]);

                    _needTimer = needCooldown;
                }
            }

            if (_needTimer > 0.0f)
            {
                _needTimer -= Time.deltaTime;
                return;
            }

            _currentTime += Time.deltaTime;
            if (_currentTime >= _needCheckFrequency)
            {
                return;
            }

            _currentTime = 0.0f;

            if (_currentMetrics.hunger < _metricsThreshold.hunger)
            {
                TryAddNeed(NeedType.Food);
            }

            if (_currentMetrics.thirst < _metricsThreshold.thirst)
            {
                TryAddNeed(NeedType.Drink);
            }

            if (_currentMetrics.enjoyment < _metricsThreshold.enjoyment)
            {
                TryAddNeed(NeedType.Music);
            }
        }

        public void TryFulfillNeed(NeedType needType, NeedMetrics metricsReward, int moodReward)
        {
            for (int i = _currentNeeds.Count - 1; i >= 0; i--)
            {
                INeed need = _currentNeeds[i];
                if (need.GetNeedType() == needType)
                {
                    _currentMetrics += metricsReward;
                    if (enableMoods)
                    {
                        mood.ChangeMood(moodReward);
                    }

                    RemoveNeed(need);

                    OnNeedFulfilled?.Invoke(need.GetNeedType());

                    _currentTime = 0.0f;
                    _needTimer = needCooldown;
                    break;
                }
            }
        }

        public bool HasUnresolvedNeed()
        {
            foreach (INeed need in _currentNeeds)
            {
                if (!needsDisplayer.IsNeedResolved(need.GetNeedType()))
                {
                    return true;
                }
            }

            return false;
        }

        public List<Message> GetUnknownNeedConversations()
        {
            var messages = new List<Message>(_currentNeeds.Count);
            foreach (INeed need in _currentNeeds)
            {
                if (!needsDisplayer.IsNeedResolved(need.GetNeedType()))
                {
                    RandomConversationSO randomConversation = need.GetRandomConversations();
                    if (randomConversation != null)
                    {
                        messages.Add(randomConversation.GetRandomMessage());
                    }
                }
            }

            return messages;
        }

        public void ResolveNeeds()
        {
            bool resolveSuccessful = needsDisplayer.ResolveNeed();
            if (resolveSuccessful)
            {
                OnNeedsResolved?.Invoke();
            }
        }

        public bool IsSatisfied()
        {
            return enableMoods && mood.IsSatisfied();
        }

        public void ChangeMood(MoodType moodType)
        {
            if (enableMoods)
            {
                mood.ChangeMood(moodType);
            }
        }

        public void ChangeMood(int moodPoints)
        {
            if (enableMoods)
            {
                mood.ChangeMood(moodPoints);
            }
        }

        private void AddNeed(INeed need)
        {
            needsDisplayer.AddDisplay(need.GetNeedType());
            _currentNeeds.Add(need);
            OnNewNeed?.Invoke(need);
        }

        private void RemoveNeed(INeed need)
        {
            needsDisplayer.RemoveDisplay(need.GetNeedType());
            _currentNeeds.Remove(need);
        }

        public void ClearNeeds()
        {
            for (int i = _currentNeeds.Count - 1; i >= 0; i--)
            {
                RemoveNeed(_currentNeeds[i]);
            }
        }

        public void TryAddNeed(NeedType needType, float startingExpirationTime = -1.0f)
        {
            if (_currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType) == null)
            {
                INeed need = GenerateNeed(needType);
                if (need != null)
                {
                    need.ResetNeed(startingExpirationTime < 0.0f ? 0.0f : startingExpirationTime);
                    AddNeed(need);
                }
            }
        }

        public void TryRemoveNeed(NeedType needType)
        {
            INeed need = _currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType);
            if (need != null)
            {
                RemoveNeed(need);
            }
        }

        [CanBeNull]
        private INeed GenerateNeed(NeedType? needType)
        {
            foreach (INeed availableNeed in _availableNeeds)
            {
                if (availableNeed.GetNeedType() == needType)
                {
                    return availableNeed;
                }
            }

            return null;
        }
    }
}