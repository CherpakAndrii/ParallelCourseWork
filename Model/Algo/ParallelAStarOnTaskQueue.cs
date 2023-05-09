using System.Collections.Concurrent;
using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ParallelAStarOnTaskQueue : IPathSearchingAlgo
{
    public ParallelAStarOnTaskQueue(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override bool SearchPath()
    {
        PriorityQueue<(int, Task<ConcurrentQueue<(int, float)>>)> verticeQueue = new PriorityQueue<(int, Task<ConcurrentQueue<(int, float)>>)>();
        Task<ConcurrentQueue<(int, float)>> calculateChildrenTask = Task.Run(() => CalculateChildren(StartPoint));
        verticeQueue.Enqueue((StartPoint, calculateChildrenTask), 0);
        Vertice currentVertice;
        while (verticeQueue.Count > 0)
        {
            (int currentVerticeIndex, calculateChildrenTask) = verticeQueue.Dequeue();
            currentVertice = _graph[currentVerticeIndex];
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return true;

            calculateChildrenTask.Wait();
            Parallel.ForEach(calculateChildrenTask.Result, indDistPair =>
            {
                Vertice child = _graph[indDistPair.Item1];
                if (!child.IsPassed && child.DistanceFromStart > indDistPair.Item2)
                {
                    child.PreviousVerticeInRouteIndex = currentVerticeIndex;
                    child.DistanceFromStart = indDistPair.Item2;
                    Task<ConcurrentQueue<(int, float)>> calculateNextChildrenTask =
                        Task.Run(async () => await CalculateChildren(indDistPair.Item1));
                    lock (verticeQueue)
                    {
                        verticeQueue.Enqueue((indDistPair.Item1, calculateNextChildrenTask),
                            child.DistanceFromStart + child.Heuristic!.Value);
                    }
                }
            });
        }

        return false;
    }

    private async Task<ConcurrentQueue<(int Vertice, float newDistance)>> CalculateChildren(int parentIndex)
    {
        Vertice parent = _graph[parentIndex];
        ConcurrentQueue<(int Vertice, float newDistance)> updateDistances =
            new ConcurrentQueue<(int Vertice, float newDistance)>();
        Parallel.ForEach(_graph.GetAdjacentVertices(parent.OwnIndex), adjIndex =>
        {
            Vertice child = _graph[adjIndex];
            if (child.IsPassed || _graph[parentIndex, child.OwnIndex] == -1)
                return;

            float newDistance = _graph[parentIndex, child.OwnIndex] + parent.DistanceFromStart;
            if (child.DistanceFromStart > newDistance)
            {
                updateDistances.Enqueue((adjIndex, newDistance));
            }
        });

        return updateDistances;
    }
}