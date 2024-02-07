using cFlowApi;
using SkiaSharp;
using System;
using WinFormsApp1;
using static cFlowForms.GuiEvents;
using static System.Net.Mime.MediaTypeNames;

namespace cFlowForms;

public class Backend
{


    private CFlowGenerator? heightmapApi;
    private BackendEventChannel backendChannel;
    private MainWindow gui;


    public Backend Populate(GuiEventChannel guiChannel, BackendEventChannel backendChannel)
    {
        this.backendChannel = backendChannel;

        guiChannel.RiverChangeRequestHandler += OnRiverChangeRequested;
        guiChannel.LoadHeightmapRequestHandler += OnHeightmapPathSelected;
        guiChannel.FlowCalculationRequestHandler += OnFlowGenerationRequested;

        return this;
    }

    private void FireLoadingEvent(bool loading)
    {
        backendChannel.RaiseLoadingState(new LoadingStateEventArgs(loading, loading ? 0 : 100));
    }

    private void FireWarning(string message)
    {
        backendChannel.RaiseMessage(new MessageEventArgs(message, MessageEventArgs.Messagetype.WARNING));
    }

    /// <summary>
    /// load new heightmap from this filepath
    /// </summary>
    /// <param name="e"></param>
    public void OnHeightmapPathSelected(object? sender, FileEventArgs e)
    {
        FireLoadingEvent(true);
        heightmapApi = new CFlowGenerator(e.FilePath);
        backendChannel.RaiseHeightmapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.HeightmapImg)));
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

        backendChannel.RaiseFlowmapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.FlowmapImgColored)));
        FireLoadingEvent(false);
    }

    public void OnRiverChangeRequested(object? sender, RiverChangeRequestEventArgs e)
    {
        if (e.ChangeType == RiverChangeType.Add)
        {
            if (heightmapApi == null)
            {
                FireWarning("no heightmap is active.");
                return;
            }
            FireLoadingEvent(true);

            heightmapApi.RiverMap.AddRiverFrom(e.pos);
            backendChannel.RaiseRivermapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.RiverMap.ToImage())));
            FireLoadingEvent(false);
        }
    }

}