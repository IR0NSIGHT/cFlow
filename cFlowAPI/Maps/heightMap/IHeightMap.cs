public interface IHeightMap: Map2d
{
    ushort GetHeight((int x, int y) pos);
    void SetHeight((int x, int y) pos, ushort z);
}
