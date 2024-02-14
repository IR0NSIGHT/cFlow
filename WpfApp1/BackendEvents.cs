namespace cFlowForms;

public readonly struct LoadingStateEventArgs(bool isLoading, int loadingProgress)
{
    public bool IsLoading { get; } = isLoading;
    public int LoadingProgress { get; } = loadingProgress;
}

public enum MapType
{
    Heightmap,
    RiverMap,
    FlowMap,
    ContourLines
}

public class ImageEventArgs(Bitmap image, MapType mapType) : EventArgs
{
    public Bitmap Image { get; } = image;
    public MapType MapType { get; } = mapType;
}

public readonly struct FileEventArgs(string filePath)
{
    public string FilePath { get; } = filePath;
}


public readonly struct MessageEventArgs(string message, MessageEventArgs.Messagetype type)
{
    public enum Messagetype
    {
        WARNING, INFO, ERROR, FATAL
    }
    public string Message { get; } = message;
    public Messagetype Type { get; } = type;
}