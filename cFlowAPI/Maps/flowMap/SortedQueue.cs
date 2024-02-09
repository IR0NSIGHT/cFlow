using static application.Maps.flowMap.SortedQueue;

namespace application.Maps.flowMap;

public class SortedQueue
{
    SortedSet<listEntry> sortedUniqueList = new(new ListEntryComparer());

    public struct listEntry
    {
        public int value;
        public (int  x, int y) point;

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

        var existingValue = new listEntry();
        var exists = sortedUniqueList.TryGetValue(newEntry, out existingValue);
        if (exists && existingValue.value > newEntry.value)
        {
            //overwrite existng value with lower value
            sortedUniqueList.Remove(existingValue);
            sortedUniqueList.Add(newEntry); 
        } else if (!exists)
        {
            //add new value
            sortedUniqueList.Add(newEntry);
        }
    }

    public listEntry Get((int x, int y) point)
    {
        var outV = new listEntry();
        var equalPoint = new listEntry() { point = point };
        var success = sortedUniqueList.TryGetValue(equalPoint, out outV);
        return outV;
    }



    public ((int x, int y) point, int value) Take()
    {
        var outV = sortedUniqueList.Min;
        sortedUniqueList.Remove(outV);
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