using System.Collections.Concurrent;
using System.Diagnostics;

namespace cFlowForms;

public class BackendEventChannel
{
    //gui wants to spawn river
    private ConcurrentQueue<Action> queue = new();

    public void Enqueue(Action a)
    {
        queue.Enqueue(a);
    }

    public void Next()
    {
        if (queue.Count <= 0) return;
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