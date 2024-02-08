using cFlowApi;
using cFlowAPI.Maps.riverMap;
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
        guiChannel.FloodChangeRequestHandler += OnFloodChangeRequested; 
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
    /// load new Heightmap from this filepath
    /// </summary>
    /// <param name="e"></param>
    public void OnHeightmapPathSelected(object? sender, FileEventArgs e)
    {
        FireLoadingEvent(true);
        heightmapApi = new CFlowGenerator(e.FilePath);
        var (shadedMap, contours) = ((Image8BitHeightMap)heightmapApi.Heightmap).ShadedHeightmap();

        backendChannel.RaiseHeightmapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(shadedMap), MapType.Heightmap));
        backendChannel.RaiseHeightmapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(contours), MapType.ContourLines));

        FireLoadingEvent(false);
    }

    public void OnFlowGenerationRequested(object? sender, EventArgs e)
    {
        if (heightmapApi == null)
        {
            FireWarning("no Heightmap is active.");
            return;
        }
        FireLoadingEvent(true);
        heightmapApi.GenerateFlow();

        backendChannel.RaiseFlowmapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.FlowmapImgColored), MapType.FlowMap));
        FireLoadingEvent(false);
    }

    public void OnRiverChangeRequested(object? sender, RiverChangeRequestEventArgs e)
    {
        if (e.ChangeType == RiverChangeType.Add)
        {
            if (heightmapApi == null)
            {
                FireWarning("no Heightmap is active.");
                return;
            }
            FireLoadingEvent(true);

            heightmapApi.RiverMap.AddRiverFrom(e.pos, e.splitEveryXBlocks);  //TODO use branching probability
            backendChannel.RaiseRivermapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.RiverMap.ToImage()), MapType.RiverMap));
            FireLoadingEvent(false);
        }
    }
    public void OnFloodChangeRequested(object? sender, FloodChangeRequestEventArgs e)
    {
        if (e.ChangeType == FloodChangeType.Add)
        {
            if (heightmapApi == null)
            {
                FireWarning("no Heightmap is active.");
                return;
            }
            FireLoadingEvent(true);

            new FloodTool(heightmapApi.Heightmap).FloodArea(
                e.pos, 
                heightmapApi.RiverMap,
                e.MaxDepth,
                e.MaxSurface
                );

            backendChannel.RaiseRivermapChanged(new ImageEventArgs(SkiaSharp.Views.Desktop.Extensions.ToBitmap(heightmapApi.RiverMap.ToImage()), MapType.RiverMap));
            FireLoadingEvent(false);
        }
    }
}