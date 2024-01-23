public interface IFlowMap
{
    record PointFlow(int X, int Y, IFlowMap.Flow Flow);
    (int x, int y) getDimensions();
    record Flow(bool Unknown, bool Up, bool Down, bool Left, bool Right)
    {
        public static String FlowToString(IFlowMap.Flow p)
        {
            if (p.Unknown)
                return "?";
            switch (p.Up ? 1 : 0, p.Down ? 1 : 0, p.Left ? 1 : 0, p.Right ? 1 : 0)
            {
                case (0, 0, 0, 0):
                    return "⊙";
                case (0, 0, 0, 1):
                    return "→";
                case (0, 0, 1, 0):
                    return "←";
                case (0, 0, 1, 1):
                    return "↔";
                case (0, 1, 0, 0):
                    return "↓";
                case (0, 1, 0, 1):
                    return "⌟";
                case (0, 1, 1, 0):
                    return "⌞";
                case (0, 1, 1, 1):  //DLR
                    return "⊤";
                case (1, 0, 0, 0):
                    return "↑";
                case (1, 0, 0, 1):
                    return "⌝";
                case (1, 0, 1, 0):
                    return "⌜";
                case (1, 0, 1, 1):  //U LR
                    return "⊥";
                case (1, 1, 0, 0):
                    return "↕";
                case (1, 1, 0, 1):  // UD R
                    return "⊢";
                case (1, 1, 1, 0):  //UP L
                    return "⊣";
                case (1, 1, 1, 1):
                    return "⊹";
                default:
                    throw new Exception("unknown flow");
            }
        }
    }

    PointFlow GetFlow(int x, int y);
    void SetFlow(int x, int y, IFlowMap.Flow flow);
    IEnumerable<PointFlow> GetPoints();
    List<PointFlow> FollowFlow(PointFlow point);

    IEnumerable<PointFlow> GetRow(int y);

    bool inBounds(int x, int y);
}
