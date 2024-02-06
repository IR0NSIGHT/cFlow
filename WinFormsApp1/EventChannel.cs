using System.Collections.Concurrent;
using System.Diagnostics;
using static cFlowForms.GuiEvents;

namespace cFlowForms;

/// <summary>
/// bridge between GUI and backend.
/// will take events from gui and fire them on a separate thread. listeners can subscribe to its events.
/// => gui doesnt get stalled by event invoking
/// => backend is one thread consuming events on after another
/// </summary>
public class EventChannel
{
    //gui wants to spawn river
    private ConcurrentQueue<Action> queue = new();

    /// <summary>
    /// will be fired on a separate thread
    /// </summary>
    public EventHandler<RiverChangeRequestEventArgs>? RiverChangeRequestHandler;
    public EventHandler? FlowCalculationRequestHandler;
    public EventHandler<FileEventArgs>? LoadHeightmapRequestHandler;

    public EventChannel()
    {
        Task.Run(ConsumerLoop);
    }   

    /// <summary>
    /// consume queued actions one by one synchronously
    /// must be run as separate thread
    /// "consume" action = fire action
    /// </summary>
    private void ConsumerLoop()
    {
        while (true)
        {
            if (queue.Count <= 0) continue;
            // Peek at the first element.
            if (!queue.TryDequeue(out var nextTask))
            {
                Debug.WriteLine("CQ: TryPeek failed when it should have succeeded");
            }
            else
            {
                nextTask();
            }
        }
    }

    public void RequestRiverChange(RiverChangeRequestEventArgs e)
    {
        //post request to queue
        queue.Enqueue(() =>
        {
            RiverChangeRequestHandler?.Invoke(this,e);
        });
    }

    public void RequestLoadHeightmap(FileEventArgs e)
    {
        //post request to queue
        queue.Enqueue(() =>
        {
            LoadHeightmapRequestHandler?.Invoke(this, e);
        });
    }

    public void RequestCalculateFlow()
    {
        queue.Enqueue(() =>
        {
            FlowCalculationRequestHandler?.Invoke(this, EventArgs.Empty);
        });
    }
}