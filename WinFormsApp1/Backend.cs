using cFlowApi;
using System;

namespace cFlowForms;

public class Backend
{
    public event EventHandler<ImageEventArgs>? HeightmapChanged;
    public event EventHandler<ImageEventArgs>? RivermapChanged;
    public event EventHandler<ImageEventArgs>? FlowmapChanged;
    public event EventHandler<LoadingStateEventArgs>? LoadingStateChanged;
    public event EventHandler<MessageEventArgs>? MessageRaised; 

    private CFlowGenerator? heightmapApi;

    private void FireLoadingEvent(bool loading)
    {
        LoadingStateChanged?.Invoke(this, new LoadingStateEventArgs(loading, loading ? 100 : 0));
    }

    private void FireWarning(string message)
    {
        MessageRaised?.Invoke(this, new MessageEventArgs(message, MessageEventArgs.Messagetype.WARNING));
    }

    /// <summary>
    /// load new heightmap from this filepath
    /// </summary>
    /// <param name="e"></param>
    protected void OnHeightmapPathSelected(FileEventArgs e)
    {
        FireLoadingEvent(true);
        heightmapApi = new CFlowGenerator(e.FilePath);
        HeightmapChanged?.Invoke(this, new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.HeightmapImg)));
        FireLoadingEvent(false);
    }

    protected void OnFlowGenerationRequested()
    {
        if (heightmapApi == null)
        {
            FireWarning("no heightmap is active.");
            return;
        }
        FireLoadingEvent(true);
        heightmapApi.GenerateFlow();

        FlowmapChanged?.Invoke(this, new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.FlowmapImgColored)));
        FireLoadingEvent(false);
    }

}