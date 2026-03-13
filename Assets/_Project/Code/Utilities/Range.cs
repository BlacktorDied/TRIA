using UnityEngine;

namespace TRIA.Utilities
{
    [System.Serializable]
    public struct IntRange
    {
        public int min;
        public int max;

        // The +1 ensures the max value is actually reachable in Random.Range
        public int RandomValue => Random.Range(min, max + 1);
    }

    [System.Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public float RandomValue => Random.Range(min, max);
    }

    // Generic version if you need it for other types
    [System.Serializable]
    public struct Range<T>
    {
        public T min;
        public T max;
    }
}
