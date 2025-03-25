using System.Collections.Generic;

namespace Rubedo.Lib;

public class WeightedList<T>
{
    private List<(T, float)> list;
    private float totalWeight;
    private Squirrel3 rnd;

    public float TotalWeight => totalWeight;

    public WeightedList()
    {
        list = new List<(T, float)>();
        rnd = new Squirrel3(System.DateTime.Now.Ticks);
    }

    public void Add(T val, float weight)
    {
        list.Add((val, weight));
        totalWeight += weight;
    }

    public void Remove(T val)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Item1.Equals(val))
            {
                totalWeight -= list[i].Item2;
                list.RemoveAt(i);
                break;
            }
        }
    }

    public T Choose()
    {
        if (list.Count == 0)
        {
            throw new System.ArgumentOutOfRangeException("Weighted list has no content.");
        }

        float selectedWeight = rnd.Range(0, totalWeight);
        int count = list.Count - 1;
        for (int i = 0; i < count; i++)
        {
            (T val, float weight) = list[i];
            selectedWeight -= weight;
            if (0 >= selectedWeight)
                return val;
        } //skip checking last one because we know it's the one.
        return list[count].Item1;
    }

    public int Count => list.Count;
}
