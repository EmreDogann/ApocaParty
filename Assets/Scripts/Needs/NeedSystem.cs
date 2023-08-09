using System;
using System.Collections.Generic;
using System.Linq;
using Dialogue;
using JetBrains.Annotations;
using MyBox;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Needs
{
    [Serializable]
    public class NeedMetrics
    {
        public float hunger;
        public float thirst;
        public float enjoyment;

        public NeedMetrics() {}

        public NeedMetrics(NeedMetrics metrics)
        {
            hunger = metrics.hunger;
            thirst = metrics.thirst;
            enjoyment = metrics.enjoyment;
        }

        public static NeedMetrics operator +(NeedMetrics a, NeedMetrics b)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = Mathf.Clamp01(a.hunger + b.hunger),
                thirst = Mathf.Clamp01(a.thirst + b.thirst),
                enjoyment = Mathf.Clamp01(a.enjoyment + b.enjoyment)
            };

            return metric;
        }

        public static NeedMetrics operator -(NeedMetrics a, NeedMetrics b)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = Mathf.Clamp01(a.hunger - b.hunger),
                thirst = Mathf.Clamp01(a.thirst - b.thirst),
                enjoyment = Mathf.Clamp01(a.enjoyment - b.enjoyment)
            };

            return metric;
        }

        public static NeedMetrics operator +(NeedMetrics a, float value)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = a.hunger + value,
                thirst = a.thirst + value,
                enjoyment = a.enjoyment + value
            };

            return metric;
        }

        public static NeedMetrics operator -(NeedMetrics a, float value)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = a.hunger - value,
                thirst = a.thirst - value,
                enjoyment = a.enjoyment - value
            };

            return metric;
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

        private float _needTimer;
        public bool TutorialMode { get; private set; }

        public event Action OnNeedsResolved;
        public event Action<INeed> OnNewNeed;
        public event Action<NeedType> OnNeedFulfilled;
        public event Action<NeedType> OnNeedExpired;

        private void Awake()
        {
            _currentNeeds = new List<INeed>();
            _availableNeeds = GetComponentsInChildren<INeed>().ToList();

            _currentMetrics = _metricsThreshold * Random.Range(0.8f, 1.2f);

            _needTimer = 0.0f;
        }

        public void Tick()
        {
            if (!TutorialMode)
            {
                _currentMetrics -= _metricsDepletionRate * Time.deltaTime;
                // if (enableMoods)
                // {
                //     mood.Tick();
                // }
            }

            for (int i = _currentNeeds.Count - 1; i >= 0; i--)
            {
                needsDisplayer.UpdateProgress(_currentNeeds[i].GetNeedType(), _currentNeeds[i].GetTimerProgress());
                if (_currentNeeds[i].IsExpired() && needsDisplayer.HasReachedProgress(_currentNeeds[i].GetNeedType(),
                        _currentNeeds[i].GetTimerProgress()))
                {
                    if (enableMoods)
                    {
                        mood.ChangeMood(-1);
                    }

                    OnNeedExpired?.Invoke(_currentNeeds[i].GetNeedType());
                    _currentMetrics += _currentNeeds[i].GetPunishment();
                    RemoveNeed(_currentNeeds[i]);

                    _needTimer = needCooldown;
                }
                else
                {
                    _currentNeeds[i].UpdateTimer(Time.deltaTime);
                }
            }

            if (_needTimer > 0.0f)
            {
                _needTimer -= Time.deltaTime;
                return;
            }

            if (TutorialMode)
            {
                return;
            }

            if (_currentMetrics.hunger < _metricsThreshold.hunger)
            {
                bool result = TryAddNeed(NeedType.Food);
                if (result)
                {
                    _needTimer = needCooldown;
                    return;
                }
            }

            if (_currentMetrics.thirst < _metricsThreshold.thirst)
            {
                bool result = TryAddNeed(NeedType.Drink);
                if (result)
                {
                    _needTimer = needCooldown;
                    return;
                }
            }

            if (_currentMetrics.enjoyment < _metricsThreshold.enjoyment)
            {
                bool result = TryAddNeed(NeedType.Music);
                if (result)
                {
                    _needTimer = needCooldown;
                }
            }
        }

        public void SetTutorialMode(bool isActive)
        {
            TutorialMode = isActive;
        }

        public void SetDepletionRate(float multiplier)
        {
            _metricsDepletionRate *= multiplier;
        }

        public void TryFulfillNeed(NeedType needType, NeedMetrics metricsReward, int moodReward)
        {
            for (int i = _currentNeeds.Count - 1; i >= 0; i--)
            {
                INeed need = _currentNeeds[i];
                if (need.GetNeedType() == needType)
                {
                    switch (needType)
                    {
                        case NeedType.Food:
                            _currentMetrics.hunger = _metricsThreshold.hunger + metricsReward.hunger;
                            break;
                        case NeedType.Drink:
                            _currentMetrics.thirst = _metricsThreshold.thirst + metricsReward.thirst;
                            break;
                        case NeedType.Music:
                            _currentMetrics.enjoyment = _metricsThreshold.enjoyment + metricsReward.enjoyment;
                            break;
                    }

                    if (enableMoods)
                    {
                        mood.ChangeMood(moodReward);
                    }

                    RemoveNeed(need);

                    OnNeedFulfilled?.Invoke(need.GetNeedType());

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

        public bool HasNeed()
        {
            return _currentNeeds.Count > 0;
        }

        public bool HasNeed(NeedType needType)
        {
            return _currentNeeds.Find(x => x.GetNeedType() == needType) != null;
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

        private void AddNeed(INeed need, bool startAsResolved)
        {
            needsDisplayer.AddDisplay(need.GetNeedType(), startAsResolved);
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

        public bool TryAddNeed(NeedType needType, float startingExpirationTime = -1.0f)
        {
            if (_currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType) == null)
            {
                INeed need = GenerateNeed(needType);
                if (need != null)
                {
                    need.ResetNeed(startingExpirationTime < 0.0f ? 0.0f : startingExpirationTime);
                    AddNeed(need);
                    return true;
                }
            }

            return false;
        }

        public bool TryAddNeed(NeedType needType, bool startAsResolved, float startingExpirationTime = -1.0f)
        {
            if (_currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType) == null)
            {
                INeed need = GenerateNeed(needType);
                if (need != null)
                {
                    need.ResetNeed(startingExpirationTime < 0.0f ? 0.0f : startingExpirationTime);
                    AddNeed(need, startAsResolved);
                    return true;
                }
            }

            return false;
        }

        public void TryRemoveNeed(NeedType needType)
        {
            INeed need = _currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType);
            if (need != null)
            {
                RemoveNeed(need);
            }
        }

        public void TryExpireNeed(NeedType needType)
        {
            INeed need = _currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType);
            need?.ExpireNeed();
        }

        public void TryResolveNeed(NeedType needType)
        {
            needsDisplayer.ResolveNeed(needType);
        }

        public void TryPauseNeed(NeedType needType)
        {
            INeed need = _currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType);
            need?.SetNeedPause(true);
        }

        public void TryUnpauseNeed(NeedType needType)
        {
            INeed need = _currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType);
            need?.SetNeedPause(false);
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