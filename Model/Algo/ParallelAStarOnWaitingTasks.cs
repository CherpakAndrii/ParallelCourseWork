using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ParallelAStarOnWaitingTasks : IPathSearchingAlgo
{
    public bool PathFound { get; set; }
    public byte NumberOfThreads { get; }
    private int _workingTasks;
    private readonly object _queueLocker;

    public ParallelAStarOnWaitingTasks(Graph graph, int startpoinIndex, int finishIndex, byte threads=12) : base(graph, startpoinIndex, finishIndex)
    {
        PathFound = false;
        NumberOfThreads = threads;
        _workingTasks = 0;
        _queueLocker = new();
    }

    public override bool SearchPath()
    {
        BlockingPriorityQueue<int> verticeQueue = new BlockingPriorityQueue<int>();
        verticeQueue.Enqueue(StartPoint, 0);
        Task[] listeners = new Task[NumberOfThreads];
        for (int i = 0; i < NumberOfThreads; i++)
        {
            listeners[i] = ListenQueue(verticeQueue);
        }

        Task.WaitAll(listeners);
        return PathFound;
    }

    private async Task ListenQueue(BlockingPriorityQueue<int> queue)
    {
        while (queue.Count > 0 || _workingTasks > 0)
        {
            Vertice current;
            while (!Monitor.TryEnter(_queueLocker))
            {
                Monitor.Wait(_queueLocker);
            }

            try
            {
                if (PathFound)
                {
                    Monitor.PulseAll(_queueLocker);
                    return;
                }
                if (queue.Count == 0) continue;
                current = _graph[queue.Dequeue()];
                Interlocked.Increment(ref _workingTasks);
            }
            finally
            {
                Monitor.PulseAll(_queueLocker);
                Monitor.Exit(_queueLocker);
            }

            if (current.IsPassed)
            {
                Interlocked.Decrement(ref _workingTasks);
                continue;
            }

            current.IsPassed = true;
            if (current.OwnIndex == EndPoint)
            {
                PathFound = true;
                Interlocked.Decrement(ref _workingTasks);
                return;
            }

            foreach (int adjIndex in _graph.GetAdjacentVertices(current.OwnIndex))
            {
                Vertice child = _graph[adjIndex];
                if (child.TryUpdateMinRoute(current.OwnIndex))
                {
                    queue.Enqueue(adjIndex, child.DistanceFromStart+child.Heuristic!.Value);
                    if (Monitor.TryEnter(_queueLocker))
                    {
                        Monitor.PulseAll(_queueLocker);
                        Monitor.Exit(_queueLocker);
                    }
                }
            }
            
            Interlocked.Decrement(ref _workingTasks);
        }
    }
}