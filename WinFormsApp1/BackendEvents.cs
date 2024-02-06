namespace cFlowForms;

public readonly struct LoadingStateEventArgs(bool isLoading, float loadingProgress)
{
    public bool IsLoading { get; } = isLoading;
    public float LoadingProgress { get; } = loadingProgress;
}


public class ImageEventArgs(Bitmap image) : EventArgs
{
    public Bitmap Image { get; } = image;
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