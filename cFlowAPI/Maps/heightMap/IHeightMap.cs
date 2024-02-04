public interface IHeightMap: Map2d
{
    short GetHeight(int x, int y);
    void SetHeight((int x, int y) pos, short z);
}
