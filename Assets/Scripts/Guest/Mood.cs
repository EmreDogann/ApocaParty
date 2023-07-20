using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

namespace Guest
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
        [Serializable]
        private class MoodThreshold
        {
            public MoodType moodType;
            public float threshold;
            public Sprite moodSprite;
        }

        [OverrideLabel("Mood Sprite Renderer")] [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private List<MoodThreshold> _moods;
        [ReadOnly] [SerializeField] private float _currentMoodLevel;
        private MoodThreshold _currentMood;

        public void Tick()
        {
            _currentMoodLevel -= 0.01f * Time.deltaTime;

            for (int i = _moods.Count - 1; i >= 0; i--)
            {
                if (_currentMoodLevel >= _moods[i].threshold)
                {
                    _currentMood = _moods[i];
                    _spriteRenderer.sprite = _moods[i].moodSprite;
                    break;
                }
            }
        }

        public void ChangeMood(MoodType newMood)
        {
            for (int i = _moods.Count - 1; i >= 0; i--)
            {
                if (_moods[i].moodType == newMood)
                {
                    _currentMoodLevel = _moods[i].threshold;
                    _currentMood = _moods[i];

                    _spriteRenderer.sprite = _moods[i].moodSprite;
                    break;
                }
            }
        }

        public void ChangeMood(int moodPoints)
        {
            _currentMoodLevel += Mathf.Clamp(moodPoints, -4, 4) / 4.0f;
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