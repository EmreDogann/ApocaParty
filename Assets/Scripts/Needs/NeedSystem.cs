using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Needs.Needs;
using PartyEvents;
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
        public float movement;

        public static NeedMetrics operator +(NeedMetrics a, NeedMetrics b)
        {
            a.hunger = Mathf.Clamp(a.hunger + b.hunger, 0.0f, 1.0f);
            a.thirst = Mathf.Clamp(a.thirst + b.thirst, 0.0f, 1.0f);
            a.enjoyment = Mathf.Clamp(a.enjoyment + b.enjoyment, 0.0f, 1.0f);
            a.movement = Mathf.Clamp(a.movement + b.movement, 0.0f, 1.0f);

            return a;
        }

        public static NeedMetrics operator -(NeedMetrics a, NeedMetrics b)
        {
            a.hunger = Mathf.Clamp(a.hunger - b.hunger, 0.0f, 1.0f);
            a.thirst = Mathf.Clamp(a.thirst - b.thirst, 0.0f, 1.0f);
            a.enjoyment = Mathf.Clamp(a.enjoyment - b.enjoyment, 0.0f, 1.0f);
            a.movement = Mathf.Clamp(a.movement - b.movement, 0.0f, 1.0f);

            return a;
        }

        public static NeedMetrics operator *(NeedMetrics a, int value)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = a.hunger * value,
                thirst = a.thirst * value,
                enjoyment = a.enjoyment * value,
                movement = a.movement * value
            };

            return metric;
        }

        public static NeedMetrics operator *(NeedMetrics a, float value)
        {
            NeedMetrics metric = new NeedMetrics
            {
                hunger = a.hunger * value,
                thirst = a.thirst * value,
                enjoyment = a.enjoyment * value,
                movement = a.movement * value
            };

            return metric;
        }

        public void SetAll(float value)
        {
            value = Mathf.Clamp(value, 0.0f, 1.0f);

            hunger = value;
            thirst = value;
            enjoyment = value;
            movement = value;
        }
    }

    public class NeedSystem : MonoBehaviour
    {
        [SerializeField] private NeedsDisplayer needsDisplayer;

        [SerializeField] private Mood mood;

        [Tooltip("The point at which the guest will generate a need to satisfy the respective metric.")]
        [MetricsRange(0.0f, 1.0f)] [SerializeField] private NeedMetrics _metricsThreshold;

        [Tooltip("The rate at which the needs metrics tick down every second.")]
        [MetricsRange(0.0f, 0.1f)] [SerializeField] private NeedMetrics _metricsDepletionRate;

        [ReadOnly] [MetricsRange(0.0f, 1.0f)] [SerializeField] private NeedMetrics _currentMetrics;
        [ReadOnly] [SerializeReference] private List<INeed> _currentNeeds;

        private readonly float _needCheckFrequency = 3.0f;
        private float _currentTime;

        private void Awake()
        {
            _currentNeeds = new List<INeed>();
            _currentMetrics = new NeedMetrics();
            _currentMetrics.SetAll(0.5f);

            _currentTime = 0.0f;
        }

        private void OnEnable()
        {
            PartyEvent.OnNeedEvent += OnPartyEvent;
        }

        private void OnDisable()
        {
            PartyEvent.OnNeedEvent -= OnPartyEvent;
        }

        private void Update()
        {
            _currentMetrics -= _metricsDepletionRate * Time.deltaTime;
            mood.Tick();

            for (int i = _currentNeeds.Count - 1; i >= 0; i--)
            {
                if (_currentNeeds[i].IsExpired())
                {
                    mood.ChangeMood(-1);
                    needsDisplayer.RemoveDisplay(_currentNeeds[i].GetNeedType());
                    _currentNeeds.RemoveAt(i);
                }
            }

            _currentTime += Time.deltaTime;
            if (_currentTime >= _needCheckFrequency)
            {
                return;
            }

            _currentTime = 0.0f;

            NeedType? needType = null;
            if (_currentMetrics.hunger < _metricsThreshold.hunger)
            {
                needType = NeedType.Food;
            }
            else if (_currentMetrics.thirst < _metricsThreshold.thirst)
            {
                needType = NeedType.Drink;
            }
            else if (_currentMetrics.enjoyment < _metricsThreshold.enjoyment)
            {
                needType = NeedType.Music;
            }
            else if (_currentMetrics.movement < _metricsThreshold.movement)
            {
                needType = NeedType.Movement;
            }

            if (_currentNeeds.FirstOrDefault(x => x.GetNeedType() == needType) == null)
            {
                INeed need = GenerateNeed(needType);
                if (need != null)
                {
                    needsDisplayer.AddDisplay(need.GetNeedType());
                    _currentNeeds.Add(need);
                }
            }
        }

        public bool IsSatisfied()
        {
            return mood.IsSatisfied();
        }

        public void ChangeMood(MoodType moodType)
        {
            mood.ChangeMood(moodType);
        }

        public void ChangeMood(int moodPoints)
        {
            mood.ChangeMood(moodPoints);
        }

        private INeed GenerateNeed(NeedType? needType)
        {
            switch (needType)
            {
                case NeedType.Food:
                    return new FoodNeed();
                // case NeedType.Drink:
                // return new DrinkNeed();
                // case NeedType.Music:
                //     return new MusicNeed();
                // case NeedType.Movement:
                //     return new MovementNeed();
                default:
                    return null;
            }
        }

        private void OnPartyEvent(NeedMetrics metric)
        {
            _currentMetrics += metric;
        }
    }
}