public class Point()
{
    public static (int x, int y) Up((int X, int Y) pos) => (pos.X + 0, pos.Y + 1);

    public static (int x, int y) Down((int X, int Y) pos) => (pos.X + 0, pos.Y - 1);
    public static (int x, int y) Left((int X, int Y) pos) => (pos.X - 1, pos.Y + 0);
    public static (int x, int y) Right((int X, int Y) pos) => (pos.X + 1, pos.Y + 0);
}
