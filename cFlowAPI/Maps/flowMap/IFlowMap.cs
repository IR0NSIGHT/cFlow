public interface IFlowMap: Map2d
{
    record PointFlow(int X, int Y, IFlowMap.Flow Flow);

    struct Flow(bool unknown, bool up, bool down, bool left, bool right)
    {
        public override string ToString()
        {
            return $"?:{unknown},u:{up},d:{down},l:{left},r:{right}";
        }

        public bool Unknown => unknown; public bool Up => up; public bool Down => down; public bool Left => left; public bool Right => right;
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

        public override bool Equals(object? obj)
        {
            if (obj is Flow flow)
            {
                return this.Down == flow.Down &&
                       this.Up == flow.Up &&
                       this.Left == flow.Left &&
                       this.Right == flow.Right &&
                       this.Unknown == flow.Unknown;
            }

            return false;
        }
    }

    Flow GetFlow((int x, int y) point);

    void SetFlow((int x, int y) point, Flow flow);

    List<(int x, int y)> FollowFlow((int x, int y) point);

  
}
