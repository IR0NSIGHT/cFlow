public record Point(int X, int Y, short height)
{
    public override string ToString()
    {
        return $"({X},{Y},{height})";
    }

    public (int x, int y) Up() => Up(X, Y) ;
    public (int x, int y) Left() => Left(X, Y);
    public (int x, int y) Down() => Down(X, Y);
    public (int x, int y) Right() => Right(X, Y);


    public static (int x, int y) Up(int X, int Y) => (X + 0, Y + 1);

    public static (int x, int y) Down(int X, int Y) => (X + 0, Y - 1);
    public static (int x, int y) Left(int X, int Y) => (X - 1, Y + 0);
    public static (int x, int y) Right(int X, int Y) => (X + 1, Y + 0);

}
