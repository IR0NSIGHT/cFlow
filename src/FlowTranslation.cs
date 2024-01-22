public static class FlowTranslation
{
    public static byte FlowToColor(IFlowMap.Flow p)
    {
        // # FF0000 (Red)
        // #00FF00 (Green)
        // #0000FF (Blue)
        // # FFFF00 (Yellow)
        // # FF00FF (Magenta)
        // #00FFFF (Cyan)
        // # FFA500 (Orange)
        // #800080 (Purple)
        // #008000 (Dark Green)
        // #008080 (Teal)
        // # FF6347 (Tomato)
        // #7CFC00 (Lawn Green)
        // #8A2BE2 (Blue Violet)
        // # FFD700 (Gold)
        // #4B0082 (Indigo)
        // # DC143C (Crimson)
        // 00CED1 Dark turquois
        if (p.Unknown)
            return (byte)0xFF;
        byte color = 0;
        color += (byte)(p.Right ? 1 : 0);
        color += (byte)(p.Left ? 2 : 0);
        color += (byte)(p.Down ? 4 : 0);
        color += (byte)(p.Up ? 8 : 0);

        return (byte)(color*15);
    }
    static String FlowToString(IFlowMap.Flow p)
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
                return "↘";
            case (0, 1, 1, 0):
                return "↙";
            case (0, 1, 1, 1):  //DLR
                return "⊤";
            case (1, 0, 0, 0):
                return "↑";
            case (1, 0, 0, 1):
                return "↗";
            case (1, 0, 1, 0):
                return "↖";
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