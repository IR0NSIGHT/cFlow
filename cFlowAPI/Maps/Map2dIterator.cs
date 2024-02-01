public class Map2dIterator : IMapIterator<(int x, int y)>
{
    private (int x, int y) bounds;
    public Map2dIterator((int x, int y) bounds)
    {
        this.bounds = bounds;
    }

    public IEnumerable<(int x, int y)> Points()
    {
        foreach (var row in PointsByRow())
            foreach (var point in row) 
                yield return point;
    }

    public IEnumerable<IEnumerable<(int x, int y)>> PointsByRow()
    {
        for (int y = 0; y < bounds.y; y++)
        {
            yield return Row(y);
        }
    }

    public IEnumerable<(int x, int y)> Row(int y)
    {
        for (int x = 0; x < bounds.x; x++)
        {
            yield return (x, y);
        }
    }
}
