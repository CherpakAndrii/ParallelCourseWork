using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ConcurrentAStar : IPathSearchingAlgo
{
    public ConcurrentAStar(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override async Task<bool> SearchPath()
    {
        PriorityQueue<int> verticeQueue = new PriorityQueue<int>();
        Vertice currentVertice;
        verticeQueue.Enqueue(StartPoint, 0);
        while (verticeQueue.Count > 0)
        {
            currentVertice = _graph[verticeQueue.Dequeue()];
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return true;

            var children = CalculateChildren(currentVertice);
            foreach(var indDistPair in children)
            {
                Vertice child = indDistPair.Item1;
                if (!child.IsPassed && child.DistanceFromStart > indDistPair.Item2)
                {
                    child.PreviousVerticeInRouteIndex = currentVertice.OwnIndex;
                    child.DistanceFromStart = indDistPair.Item2;
                    verticeQueue.Enqueue(child.OwnIndex,
                            child.DistanceFromStart + child.Heuristic!.Value);
                }
            }
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
            Interlocked.Increment(ref ChildrenCalculatedCounter);
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
