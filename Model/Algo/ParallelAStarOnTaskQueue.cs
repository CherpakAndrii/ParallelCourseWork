using System.Collections.Concurrent;
using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ParallelAStarOnTaskQueue : IPathSearchingAlgo
{
    public ParallelAStarOnTaskQueue(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public async override Task<bool> SearchPath()
    {
        PriorityQueue<(Vertice, Task<List<(Vertice, float)>>)> verticeQueue = new PriorityQueue<(Vertice, Task<List<(Vertice, float)>>)>();
        Vertice currentVertice = _graph[StartPoint];
        Task<List<(Vertice, float)>> calculateChildrenTask = Task.Run( () => CalculateChildren(currentVertice));
        verticeQueue.Enqueue((currentVertice, calculateChildrenTask), 0);
        while (verticeQueue.Count > 0)
        {
            (currentVertice, calculateChildrenTask) = verticeQueue.Dequeue();
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return true;

            var children = await calculateChildrenTask;
            Parallel.ForEach(children, indDistPair =>
            {
                Vertice child = indDistPair.Item1;
                lock (child)
                    if (!child.IsPassed && child.DistanceFromStart > indDistPair.Item2)
                    {
                        child.PreviousVerticeInRouteIndex = currentVertice.OwnIndex;
                        child.DistanceFromStart = indDistPair.Item2;
                        Task<List<(Vertice, float)>> calculateNextChildrenTask =
                            Task.Run(() => CalculateChildren(indDistPair.Item1));
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

    private List<(Vertice vertice, float newDistance)> CalculateChildren(Vertice parent)
    {
        List<(Vertice vertice, float newDistance)> updateDistances =
            new List<(Vertice vertice, float newDistance)>();
        foreach (var adjIndex in _graph.GetAdjacentVertices(parent.OwnIndex))
        {
            Vertice child = _graph[adjIndex];
            if (child.IsPassed)
                continue;

            float newDistance = _graph[parent.OwnIndex, child.OwnIndex] + parent.DistanceFromStart;
            if (child.DistanceFromStart > newDistance)
            {
                updateDistances.Add((child, newDistance));
            }
        }

        return updateDistances;
    }
}