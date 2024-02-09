using static application.Maps.flowMap.SortedQueue;

namespace application.Maps.flowMap;

public class SortedQueue
{
    private List<listEntry> sortedList = new();
    private HashSet<(int x, int y)> knowPoints = new();

    public struct listEntry
    {
        public int value;
        public (int x, int y) point;

        public override string ToString()
        {
            return $"{point}:{value}";
        }
    }

    /// <summary>
    /// will insert if: point is unknown OR point is know with a higher value
    /// list is guaranteed unique on point coords
    /// </summary>
    /// <param name="point"></param>
    /// <param name="value"></param>
    public void TryInsert((int x, int y) point, int value)
    {
        var newEntry = new listEntry() { point = point, value = value };
        var existingValue = GetValue(point);
        if (existingValue.value == -1)
        {
            sortedList.Add(new listEntry() { point = point, value = value });
            knowPoints.Add(point);
            sortedList.Sort((a, b) => a.value.CompareTo(b.value));
        }
        else if (existingValue.value > newEntry.value)
        {
            //overwrite existng value with lower value / or insert new value
            sortedList.Remove(existingValue);
            sortedList.Add(new listEntry() { point = point, value = value });
            sortedList.Sort((a, b) => a.value.CompareTo(b.value));
        }
    }

    public listEntry GetValue((int x, int y) point)
    {
        if (knowPoints.Contains(point))
            return sortedList.Find(p => p.point == point);
        else
            return new listEntry() { value = -1 };
    }

    public bool isEmpty()
    {
        return sortedList.Count == 0;
    }


    public ((int x, int y) point, int value) Take()
    {
        var outV = sortedList[0];
        sortedList.Remove(outV);
        return (outV.point, outV.value);
    }
}


// Custom comparer for listEntry
internal class ListEntryComparer : IComparer<listEntry>
{
    // Compare method for sorting
    public int Compare(listEntry x, listEntry y)
    {
        return x.point.CompareTo(y.point);
    }
}