using System.Collections.Generic;
using UnityEngine;

public static class CUtil
{
    public const float MAP_HIGHT_MAX = 0.6F;
    public const float MAP_HIGHT_MIN = -4.5F;
    public const float MAP_WITH_MIN = -10f;
    public const float MAP_WITH_MAX = 50f;
}

[System.Serializable]
public class CWeightedRandom<T>
{
    [System.Serializable]
    public struct SRandomWeight
    {
        public T Item;
        public float Weight;
    }

    public List<SRandomWeight> RandomWeightList;

    public T GetRandomItem()
    {
        float totalWeight = 0;

        for (int i = 0; i < RandomWeightList.Count; i++)
        {
            totalWeight += RandomWeightList[i].Weight;
        }

        float ranNum = Random.Range(0, totalWeight);
        float weightedWeight = 0;

        for (int i = 0; i < RandomWeightList.Count; i++)
        {
            weightedWeight += RandomWeightList[i].Weight;

            if (weightedWeight <= ranNum)
            {
                return RandomWeightList[i].Item;
            }
        }

        return RandomWeightList[RandomWeightList.Count - 1].Item;
    }
}