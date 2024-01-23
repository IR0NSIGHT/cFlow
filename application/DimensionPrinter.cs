using System.Text;

public class DimensionPrinter
{
    public static String DimensionToString<T>(
        IEnumerable<IEnumerable<T>> it,
        Func<T, String> toString,
        String itemSeparator = "\t",
        String lineSeparator = "\n"
        )
    {
        return String.Join(lineSeparator,
            it.Select(
                line => String.Join(itemSeparator,
                line.Select(point => toString(point)))));
    }
}


public class FlowMapPrinter
{
    public static String FlowMapToString(IFlowMap flowMap, IHeightMap heightMap)
    {
        StringBuilder all = new StringBuilder();

        for (int y = flowMap.getDimensions().y - 1; y >= 0; y --)
        {
            StringBuilder topLine = new StringBuilder();
            StringBuilder middleLine = new StringBuilder();
            StringBuilder bottomLine = new StringBuilder();

            for (int x = 0; x < flowMap.getDimensions().x; x ++) {
                var flowPoint = flowMap.GetFlow(x, y);
                var height = heightMap.GetHeight(x, y);
                String heightS = height.ToString().PadLeft(3);
                if (flowPoint.Flow.Unknown)
                {
                    topLine.Append("     ");
                    middleLine.Append($" {heightS}?");
                    bottomLine.Append("     ");
                } else
                {

                    String up = flowPoint.Flow.Up ? "  ↑  " : "     ";
                    String down = flowPoint.Flow.Down ? "  ↓  " : "     ";
                    topLine.Append(up);
                    bottomLine.Append(down);

                    var left = flowPoint.Flow.Left ? '←' : ' ';
                    var right = flowPoint.Flow.Right ? '→' : ' ';
                    String pointS = $"{left}{heightS}{right}";
                    middleLine.Append(pointS);
                }
            }

            all.AppendLine(topLine.ToString() );
            all.AppendLine(middleLine.ToString() );
            all.AppendLine(bottomLine.ToString() ); 
        }
        return all.ToString();
    }
}