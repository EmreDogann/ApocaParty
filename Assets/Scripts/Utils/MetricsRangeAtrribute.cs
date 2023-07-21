using UnityEngine;

namespace Utils
{
    public class MetricsRangeAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;

        public MetricsRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}