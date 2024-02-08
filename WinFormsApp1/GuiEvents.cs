namespace cFlowForms;

public class GuiEvents
{
    public enum RiverChangeType
    {
        Add,
        Remove
    }
    public readonly record struct RiverChangeRequestEventArgs((int x, int y) pos, RiverChangeType ChangeType)
    {
        public (int x, int y) Position { get; } = pos;
        public RiverChangeType ChangeType { get; } = ChangeType;

    }


    public enum FloodChangeType
    {
        Add,
        Remove
    }

    public readonly record struct FloodChangeRequestEventArgs((int x, int y) pos, FloodChangeType ChangeType)
    {
        public (int x, int y) Position { get; } = pos;
        public FloodChangeType ChangeType { get; } = ChangeType;
    }
}