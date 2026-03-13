using UnityEngine;

[System.Serializable]
public struct Range<T>
{
    public T min;
    public T max;

    public Range(T min, T max)
    {
        this.min = min;
        this.max = max;
    }
}

[System.Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public int RandomValue => Random.Range(min, max + 1);
}

[System.Serializable]
public struct FloatRange
{
    public float min;
    public float max;

    public float RandomValue => Random.Range(min, max);
}
