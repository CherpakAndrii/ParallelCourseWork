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
        Task<ConcurrentQueue<(int, float)>> calculateChildrenTask = CalculateChildren(StartPoint);
        //calculateChildrenTask.Start();
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

            foreach ((int adjIndex, float distance) in calculateChildrenTask.Result)
            {
                Vertice child = _graph[adjIndex];
                if (!child.IsPassed && child.DistanceFromStart > distance)
                {
                    child.PreviousVerticeInRouteIndex = currentVerticeIndex;
                    child.DistanceFromStart = distance;
                    Task<ConcurrentQueue<(int, float)>> calculateNextChildrenTask = CalculateChildren(adjIndex);
                    //calculateNextChildrenTask.Start();
                    
                    verticeQueue.Enqueue((adjIndex, calculateNextChildrenTask), child.DistanceFromStart+child.Heuristic!.Value);
                }
            }
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