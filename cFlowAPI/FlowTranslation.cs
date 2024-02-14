using System.Drawing;

public static class FlowTranslation
{
    public static Color FlowToColor(IFlowMap.Flow p)
    {
        if (p.Unknown)
            return Color.Red;

        switch (p.Up ? 1 : 0, p.Down ? 1 : 0, p.Left ? 1 : 0, p.Right ? 1 : 0)
        {
            case (0, 0, 0, 0):
                return Color.Black;
            case (0, 0, 0, 1):
                return Color.Green;
            case (0, 0, 1, 0):
                return Color.MediumOrchid;
            case (0, 0, 1, 1):
                return Color.Gold;
            case (0, 1, 0, 0):
                return Color.LightBlue;
            case (0, 1, 0, 1):
                return Color.CadetBlue;
            case (0, 1, 1, 0):
                return Color.Violet;
            case (0, 1, 1, 1):  //DLR
                return Color.Gold;
            case (1, 0, 0, 0):
                return Color.Orange;
            case (1, 0, 0, 1):
                return Color.Yellow;
            case (1, 0, 1, 0):
                return Color.Pink;
            case (1, 0, 1, 1):  //U LR
                return Color.Gold;
            case (1, 1, 0, 0):
                return Color.Gold;
            case (1, 1, 0, 1):  // UD R
                return Color.Gold;
            case (1, 1, 1, 0):  //UP L
                return Color.Gold;
            case (1, 1, 1, 1):
                return Color.Gold;
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