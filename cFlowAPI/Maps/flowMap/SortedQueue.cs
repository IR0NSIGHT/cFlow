using static application.Maps.flowMap.SortedQueue;

namespace application.Maps.flowMap;

public class SortedQueue
{
    private PriorityQueue<(int x, int y), int> sortedList = new();
    private Dictionary<(int x, int y), int> knowPoints = new();

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
        var isKnown = knowPoints.ContainsKey(point);
        var knownValue = isKnown ? knowPoints[point] : -1;
        if (isKnown && knownValue > value)
        {
            knowPoints[point] = value;
        } else if (!isKnown)
        {
            knowPoints.Add(point, value);
            sortedList.Enqueue(point, value); //accept that point is in there twice now
        }
    }

    public bool isEmpty()
    {
        return sortedList.Count == 0;
    }

    private HashSet<(int x, int y)> taken = new HashSet<(int x, int y)>();

    public ((int x, int y) point, int value) Take()
    {
        var outV = sortedList.Dequeue();
        if (taken.Contains(outV))
            return Take();
        taken.Add(outV);
        var val = knowPoints.GetValueOrDefault(outV, -1);
        return (outV, val);
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