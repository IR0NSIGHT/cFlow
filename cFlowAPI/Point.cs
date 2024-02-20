public class Point()
{
    public static int DistanceSquared((int x, int y) p1, (int x, int y) p2)
    {
        var delta = Delta(p1, p2);
        return delta.Item1*delta.Item1 + delta.Item2*delta.Item2;
    }

    public static (int x, int y) Delta((int x, int y) p1, (int x, int y) p2)
    {
        return (p1.x - p2.x, p1.y - p2.y);
    }

    public static (int x, int y) Up((int X, int Y) pos) => (pos.X + 0, pos.Y + 1);

    public static (int x, int y) Down((int X, int Y) pos) => (pos.X + 0, pos.Y - 1);
    public static (int x, int y) Left((int X, int Y) pos) => (pos.X - 1, pos.Y + 0);
    public static (int x, int y) Right((int X, int Y) pos) => (pos.X + 1, pos.Y + 0);

    public static (int x, int y)[] Neighbours((int X, int Y) pos) => 
        [Up(pos), Down(pos), Left(pos), Right(pos)];
    public static (int x, int y)[] Diagonal((int X, int Y) pos) => 
        [Up(Right(pos)), Up(Left(pos)), Down(Right(pos)), Down(Left(pos))];

}
