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

    public async override Task<bool> SearchPath()
    {
        BlockingPriorityQueue<int> verticeQueue = new BlockingPriorityQueue<int>();
        verticeQueue.Enqueue(StartPoint, 0);
        Task[] listeners = new Task[NumberOfThreads];
        for (int i = 0; i < NumberOfThreads; i++)
        {
            listeners[i] = Task.Run(() => ListenQueue(verticeQueue));
        }

        foreach (var task in listeners)
        {
            await task;
        }
        return PathFound;
    }

    private async Task ListenQueue(BlockingPriorityQueue<int> queue)
    {
        Vertice current;
        while ((queue.Count > 0 || _workingTasks > 0) && !PathFound)
        {
            lock (_queueLocker)
            {
                if (queue.Count == 0)
                {
                    Monitor.Wait(_queueLocker);
                    continue;
                }
                current = _graph[queue.Dequeue()];
                Interlocked.Increment(ref _workingTasks);
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
                    lock (_queueLocker)
                    {
                        queue.Enqueue(adjIndex, child.DistanceFromStart+child.Heuristic!.Value);
                        Monitor.PulseAll(_queueLocker);
                    }
                }
            }
            
            Interlocked.Decrement(ref _workingTasks);
        }
        lock (_queueLocker)
        {
            Monitor.PulseAll(_queueLocker);
        }
    }
}