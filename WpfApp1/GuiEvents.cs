﻿namespace cFlowForms;

public class GuiEvents
{
    public enum RiverChangeType
    {
        Add,
        Remove
    }
    public record struct RiverChangeRequestEventArgs((int x, int y) pos, RiverChangeType ChangeType)
    {
        public (int x, int y) Position { get; } = pos;
        public RiverChangeType ChangeType { get; } = ChangeType;
        public int splitEveryXBlocks;
    }


    public enum FloodChangeType
    {
        Add,
        Remove
    }

    public record struct FloodChangeRequestEventArgs((int x, int y) pos, FloodChangeType ChangeType, int maxDepth = 10, int maxSurface = 100000)
    {
        public (int x, int y) Position { get; } = pos;
        public FloodChangeType ChangeType { get; } = ChangeType;
        public int MaxDepth { get; } = maxDepth;
        public int MaxSurface { get; } = maxSurface;
    }
}