public static class FlowTranslation
{
    public static byte FlowToGray8(IFlowMap.Flow p)
    {
       
        if (p.Unknown)
            return (byte)0xFF;
        byte color = 0;
        color += (byte)(p.Right ? 1 : 0);
        color += (byte)(p.Left ? 2 : 0);
        color += (byte)(p.Down ? 4 : 0);
        color += (byte)(p.Up ? 8 : 0);

        return (byte)(color*7);
    }

    public static uint FlowToColor(IFlowMap.Flow p)
    {
        List<uint> colorUIntValues = new List<uint>
        {
            0xFF0000FF, // Red
            0x00FF00FF, // Green
            0x0000FFFF, // Blue
            0xFFFF00FF, // Yellow
            0xFF00FFFF, // Magenta
            0x00FFFFFF, // Cyan
            0xFFA500FF, // Orange
            0x800080FF, // Purple
            0x008000FF, // Dark Green
            0x008080FF, // Teal
            0xFF6347FF, // Tomato
            0x7CFC00FF, // Lawn Green
            0x8A2BE2FF, // Blue Violet
            0xFFD700FF, // Gold
            0x4B0082FF, // Indigo
            0xDC143CFF, // Crimson
            0x00CED1FF  // Dark Turquoise
        };

        if (p.Unknown)
            return colorUIntValues[0];
        switch (p.Up ? 1 : 0, p.Down ? 1 : 0, p.Left ? 1 : 0, p.Right ? 1 : 0)
        {
            case (0, 0, 0, 0):
                return colorUIntValues[0];
            case (0, 0, 0, 1):
                return colorUIntValues[1];
            case (0, 0, 1, 0):
                return colorUIntValues[2];
            case (0, 0, 1, 1):
                return colorUIntValues[3];
            case (0, 1, 0, 0):
                return colorUIntValues[4];
            case (0, 1, 0, 1):
                return colorUIntValues[5];
            case (0, 1, 1, 0):
                return colorUIntValues[6];
            case (0, 1, 1, 1):  //DLR
                return colorUIntValues[7];
            case (1, 0, 0, 0):
                return colorUIntValues[8];
            case (1, 0, 0, 1):
                return colorUIntValues[9];
            case (1, 0, 1, 0):
                return colorUIntValues[10];
            case (1, 0, 1, 1):  //U LR
                return colorUIntValues[11];
            case (1, 1, 0, 0):
                return colorUIntValues[12];
            case (1, 1, 0, 1):  // UD R
                return colorUIntValues[13];
            case (1, 1, 1, 0):  //UP L
                return colorUIntValues[14];
            case (1, 1, 1, 1):
                return colorUIntValues[15];
            default:
                throw new Exception("unknown flow");
        }
       
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