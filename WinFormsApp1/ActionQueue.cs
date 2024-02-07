using System.Collections.Concurrent;
using System.Diagnostics;

namespace cFlowForms;

public class ActionQueue
{
    private Task runner;
    private bool doRun;

    public ActionQueue Start()
    {
        runner = Task.Run(ConsumerLoop);
        return this;
    }

    public ActionQueue Stop()
    {
        doRun = false;
        return this;
    }

    //gui wants to spawn river
    private ConcurrentQueue<Action> queue = new();

    /// <summary>
    /// consume queued actions one by one synchronously
    /// must be run as separate thread
    /// "consume" action = fire action
    /// </summary>
    private void ConsumerLoop()
    {
        while (doRun)
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

    public void Enqueue(Action action)
    {
        //post request to queue
        queue.Enqueue(action);
    }
}