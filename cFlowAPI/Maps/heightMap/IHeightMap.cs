public interface IHeightMap: Map2d
{
    short GetHeight(int x, int y);
    void SetHeight((int x, int y) pos, short z);
}

public interface Map2d
{
    (int x, int y) Bounds();
    bool inBounds(int x, int y);

    IMapIterator<(int x, int y)> iterator();
}