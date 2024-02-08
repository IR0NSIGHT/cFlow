namespace cFlowForms;

public class LayerProvider
{
    public LayerProvider()
    {
        AddLayer(new Bitmap(1, 1), "Heightmap");
        AddLayer(new Bitmap(1, 1), "Flow");
        AddLayer(new Bitmap(1, 1), "Water");
        AddLayer(new Bitmap(1, 1), "Contour");
    }

    public static readonly int HeightmapLayer = 0;
    public static readonly int FlowLayer = 1;
    public static readonly int RiverLayer = 2;
    public static readonly int ContourLayer = 3;

    public EventHandler LayerToggledEventHandler;

    private List<(Bitmap image, bool active, string name)> layers = new();

    public IEnumerable<Bitmap> ActiveLayers()
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

    private void AddLayer(Bitmap layer, string name)
    {
        layers.Add((layer, true, name));
    }

    private (Bitmap, bool, string) GetLayer(int index)
    {
        return layers[index];
    }

    public void UpdateLayerBitmap(int index, Bitmap layer)
    {
        var ex = GetLayer(index);

        layers[index] = (layer, ex.Item2, ex.Item3);
    }

    public bool IsLayerActive(int index)
    {
        return layers[index].active;
    }
    public void ToggleLayer(int idx)
    {
        var (layer, active, name) = layers[idx];
        layers[idx] = (layer, !active, name);

        LayerToggledEventHandler.Invoke(this, EventArgs.Empty);
    }
}