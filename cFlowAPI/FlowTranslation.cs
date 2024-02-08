using SkiaSharp;

public static class FlowTranslation
{
    public static class FlowColorsARGB
    {
        public static readonly SKColor Red = 0xFFFF0000;
        public static readonly SKColor Green = 0xFF00FF00;
        public static readonly SKColor LightBlue = 0xFFadd8e6;
        public static readonly SKColor Yellow = 0xFFFFFF00;
        public static readonly SKColor Magenta = 0xFFFF00FF;
        public static readonly SKColor Cyan = 0xFF00FFFF;
        public static readonly SKColor Orange = 0xFFFFA500;
        public static readonly SKColor Purple = 0xFF800080;
        public static readonly SKColor DarkGreen = 0xFF008000;
        public static readonly SKColor Teal = 0xFF008080;
        public static readonly SKColor Tomato = 0xFFFF6347;
        public static readonly SKColor LawnGreen = 0xFF7CFC00;
        public static readonly SKColor Violet = 0xFF8A2BE2;
        public static readonly SKColor Gold = 0xFFFFD700;
        public static readonly SKColor Indigo = 0xFF4B0082;
        public static readonly SKColor Crimson = 0xFFDC143C;
        public static readonly SKColor DarkTurquoise = 0xFF00CED1;
        public static readonly SKColor Pink = 0xFFFFC0CB;
        public static readonly SKColor Mauves = 0xffe0b0ff;
        public static readonly SKColor BlueGreen = 0xff7bccb5;

    }


    public static byte FlowToGray8(IFlowMap.Flow p)
    {

        if (p.Unknown)
            return (byte)0xFF;
        byte color = 0;
        color += (byte)(p.Right ? 1 : 0);
        color += (byte)(p.Left ? 2 : 0);
        color += (byte)(p.Down ? 4 : 0);
        color += (byte)(p.Up ? 8 : 0);

        return (byte)(color * 7);
    }

    public static SKColor FlowToColor(IFlowMap.Flow p)
    {
        if (p.Unknown)
            return FlowColorsARGB.Red;

        switch (p.Up ? 1 : 0, p.Down ? 1 : 0, p.Left ? 1 : 0, p.Right ? 1 : 0)
        {
            case (0, 0, 0, 0):
                return FlowColorsARGB.Red;
            case (0, 0, 0, 1):
                return FlowColorsARGB.Green;
            case (0, 0, 1, 0):
                return FlowColorsARGB.Mauves;
            case (0, 0, 1, 1):
                return FlowColorsARGB.Red;
            case (0, 1, 0, 0):
                return FlowColorsARGB.LightBlue;
            case (0, 1, 0, 1):
                return FlowColorsARGB.BlueGreen;
            case (0, 1, 1, 0):
                return FlowColorsARGB.Violet;
            case (0, 1, 1, 1):  //DLR
                return FlowColorsARGB.Red;
            case (1, 0, 0, 0):
                return FlowColorsARGB.Orange;
            case (1, 0, 0, 1):
                return FlowColorsARGB.Yellow;
            case (1, 0, 1, 0):
                return FlowColorsARGB.Pink;
            case (1, 0, 1, 1):  //U LR
                return FlowColorsARGB.Red;
            case (1, 1, 0, 0):
                return FlowColorsARGB.Red;
            case (1, 1, 0, 1):  // UD R
                return FlowColorsARGB.Red;
            case (1, 1, 1, 0):  //UP L
                return FlowColorsARGB.Red;
            case (1, 1, 1, 1):
                return FlowColorsARGB.Gold;
            default:
                throw new Exception("Unknown flow");
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