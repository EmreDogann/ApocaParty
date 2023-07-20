using System;
using System.Collections.Generic;
using UnityEngine;

namespace Needs
{
    [Serializable]
    public class MoodThreshold
    {
        public MoodType moodType;
        public float threshold;
        public Sprite moodSprite;
    }

    [CreateAssetMenu(menuName = "Mood/New Mood Definition", fileName = "New Mood Definition", order = 0)]
    public class MoodDefinitionSO : ScriptableObject
    {
        [SerializeField] private List<MoodThreshold> _moodThresholds;

        public List<MoodThreshold> GetMoods()
        {
            return _moodThresholds;
        }
    }
}