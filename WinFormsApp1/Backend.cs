using cFlowApi;
using System;
using static cFlowForms.GuiEvents;

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
    public void OnHeightmapPathSelected(object? sender, FileEventArgs e)
    {
        FireLoadingEvent(true);
        heightmapApi = new CFlowGenerator(e.FilePath);
        HeightmapChanged?.Invoke(this, new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.HeightmapImg)));
        FireLoadingEvent(false);
    }

    public void OnFlowGenerationRequested(object? sender, EventArgs e)
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

    public async void OnRiverChangeRequested(object? sender, RiverChangeRequestEventArgs e)
    {
        if (e.ChangeType == RiverChangeType.Add)
        {
            if (heightmapApi == null)
            {
                FireWarning("no heightmap is active.");
                return;
            }
            FireLoadingEvent(true);
            await Task.Run(() =>
            {
                Task.Delay(10000);
                heightmapApi.RiverMap.AddRiverFrom(e.pos);
                RivermapChanged?.Invoke(this, new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.RiverMap.ToImage())));
                FireLoadingEvent(false);
            });


        }
    }

}