using System;
using MyBox;
using UnityEngine;

namespace Needs
{
    public enum MoodType
    {
        Angry,
        Sad,
        Fine,
        Happy
    }

    [Serializable]
    public class Mood
    {
        [OverrideLabel("Mood Sprite Renderer")] [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MoodDefinitionSO _moodsDefinition;
        [ReadOnly] [SerializeField] private float _currentMoodLevel;
        private MoodThreshold _currentMood;
        [ReadOnly] [SerializeField] private MoodType _currentMoodType;

        internal void Tick()
        {
            _currentMoodLevel -= 0.01f * Time.deltaTime;
            _currentMoodLevel = Mathf.Clamp(_currentMoodLevel, 0.0f, 1.0f);

            // For debugging only.
            _currentMoodType = _currentMood.moodType;

            for (int i = _moodsDefinition.GetMoods().Count - 1; i >= 0; i--)
            {
                MoodThreshold mood = _moodsDefinition.GetMoods()[i];
                if (_currentMoodLevel >= mood.threshold)
                {
                    _currentMood = mood;
                    _spriteRenderer.sprite = mood.moodSprite;
                    break;
                }
            }
        }

        public void ChangeMood(MoodType newMood)
        {
            for (int i = _moodsDefinition.GetMoods().Count - 1; i >= 0; i--)
            {
                MoodThreshold mood = _moodsDefinition.GetMoods()[i];
                if (mood.moodType == newMood)
                {
                    _currentMoodLevel = mood.threshold;
                    _currentMood = mood;

                    _spriteRenderer.sprite = mood.moodSprite;
                    break;
                }
            }
        }

        public void ChangeMood(int moodPoints)
        {
            _currentMoodLevel += Mathf.Clamp(moodPoints, -4, 4) / 4.0f;
            _currentMoodLevel = Mathf.Clamp(_currentMoodLevel, 0.0f, 1.0f);
        }

        public MoodType GetCurrentMood()
        {
            return _currentMood.moodType;
        }

        public bool IsSatisfied()
        {
            return _currentMood.moodType is MoodType.Happy or MoodType.Fine;
        }
    }
}