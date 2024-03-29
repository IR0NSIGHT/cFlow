﻿using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.components;

namespace cFlowForms;

public class LayerProvider
{
    public LayerProvider(StackPanel buttonContainerControl)
    {
        AddLayer(FromBitmap(new Bitmap(1, 1)), "Heightmap");
        AddLayer(FromBitmap(new Bitmap(1, 1)), "Flow");
        AddLayer(FromBitmap(new Bitmap(1, 1)), "Water");
        AddLayer(FromBitmap(new Bitmap(1, 1)), "Contour");
        CreateButtons(buttonContainerControl);
    }

    public static readonly int HeightmapLayer = 0;
    public static readonly int FlowLayer = 1;
    public static readonly int RiverLayer = 2;
    public static readonly int ContourLayer = 3;

    public EventHandler LayerToggledEventHandler;

    private List<(ImageSource image, bool active, string name)> layers = new();
    private ToolButton[] _layerToggleButtons = Array.Empty<ToolButton>();
    public IEnumerable<ImageSource> ActiveLayers()
    {
        foreach (var (layer, active, _) in layers)
        {
            if (active)
                yield return layer;
        }
    }

    public IEnumerable<(string name, bool active, int idx)> AllLayers()
    {
        return layers.Select((x, i) => (x.name, x.active, i));
    }

    private void AddLayer(ImageSource layer, string name)
    {
        layers.Add((layer, true, name));
    }

    private void CreateButtons(StackPanel buttonContainerControl)
    {
        _layerToggleButtons = new ToolButton[layers.Count];
        buttonContainerControl.Children.Clear();
        foreach (var tuple in AllLayers())
        {
            var button = new ToolButton();
            _layerToggleButtons[tuple.idx] = button;
            button.SetActive(tuple.active);
            button.ToolName = tuple.name;
            var callBack = new EventHandler<bool>((sender, newState) =>
            {
                
                ToggleLayer(tuple.idx, newState);
            });
            button.OnToggledEventHandler += callBack;

            buttonContainerControl.Children.Add(button);
        }
    }

    private (ImageSource, bool, string) GetLayer(int index)
    {
        return layers[index];
    }

    public void UpdateLayerBitmap(int index, Bitmap layer)
    {
        var ex = GetLayer(index);
        layers[index] = (FromBitmap(layer), ex.Item2, ex.Item3);
    }
    private BitmapSource FromBitmap(Bitmap bmp)
    {
        IntPtr hBitmap = bmp.GetHbitmap();
        BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()
        );
        return bitmapSource;
    }
    public bool IsLayerActive(int index)
    {
        return layers[index].active;
    }
    public void ToggleLayer(int idx, bool newState)
    {
        var (layer, active, name) = layers[idx];
        layers[idx] = (layer, newState, name);

        LayerToggledEventHandler?.Invoke(this, EventArgs.Empty);
    }
}