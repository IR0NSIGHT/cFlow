public class Point()
{
    public static (int x, int y) Up((int X, int Y) pos) => (pos.X + 0, pos.Y + 1);

    public static (int x, int y) Down((int X, int Y) pos) => (pos.X + 0, pos.Y - 1);
    public static (int x, int y) Left((int X, int Y) pos) => (pos.X - 1, pos.Y + 0);
    public static (int x, int y) Right((int X, int Y) pos) => (pos.X + 1, pos.Y + 0);

    public static (int x, int y)[] Neighbours((int X, int Y) pos) => 
        [Up(pos), Down(pos), Left(pos), Right(pos)];
    public static (int x, int y)[] Diagonal((int X, int Y) pos) => 
        [Up(Right(pos)), Up(Left(pos)), Down(Right(pos)), Down(Left(pos))];

}
