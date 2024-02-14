using System.Windows;
using WpfApp1;

namespace cFlowForms;
/// <summary>
/// event channel that will invoke all EventHandlers on the graphics thread
/// => backend requests event to be fired, channel fires it on the graphics thread
/// </summary>
public class BackendEventChannel
{
    private MainWindow gui;
    public event EventHandler<ImageEventArgs>? HeightmapChanged;
    public event EventHandler<ImageEventArgs>? RivermapChanged;
    public event EventHandler<ImageEventArgs>? FlowmapChanged;
    public event EventHandler<MessageEventArgs>? MessageRaised;
    public event EventHandler<LoadingStateEventArgs>? LoadingStateChanged;

    public BackendEventChannel(MainWindow gui)
    {
        this.gui = gui;
    }

    public void RaiseHeightmapChanged(ImageEventArgs args)
    {
        RunOnGuiThread(() => HeightmapChanged?.Invoke(this, args));
    }

    public void RaiseFlowmapChanged(ImageEventArgs args)
    {
        RunOnGuiThread(() => FlowmapChanged?.Invoke(this, args));
    }

    public void RaiseRivermapChanged(ImageEventArgs args)
    {
        RunOnGuiThread(() => RivermapChanged?.Invoke(this, args));
    }

    public void RaiseMessage(MessageEventArgs args)
    {
        RunOnGuiThread(() => MessageRaised?.Invoke(this, args));
    }

    public void RaiseLoadingState(LoadingStateEventArgs args)
    {
        RunOnGuiThread(() => LoadingStateChanged?.Invoke(this, args));
    }

    private void RunOnGuiThread(Action action)
    {
        //will execute the Event listeners on the graphics thread
        Action safeLoadingStateUpdate = delegate
        {
            action();
        };
        Application.Current.Dispatcher.Invoke(safeLoadingStateUpdate);
    }
}