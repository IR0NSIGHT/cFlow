public interface Map2d
{
    (int x, int y) Bounds();
    bool inBounds(int x, int y);

    IMapIterator<(int x, int y)> iterator();
}