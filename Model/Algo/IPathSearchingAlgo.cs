using Model.Entities;

namespace Model.Algo;

public abstract class IPathSearchingAlgo
{
    public int ChildrenCalculatedCounter;
    protected int StartPoint { get; }
    protected int EndPoint { get; }
    protected Graph _graph { get; }
    protected IPathSearchingAlgo(Graph graph, int startpoinIndex, int finishIndex)
    {
        StartPoint = startpoinIndex;
        EndPoint = finishIndex;
        graph.SetEndPoint(finishIndex);
        graph[startpoinIndex].DistanceFromStart = 0;
        _graph = graph;
        ChildrenCalculatedCounter = 0;
    }

    public abstract Task<bool> SearchPath();

    public Stack<int> TraceRoute()
    {
        Stack<int> route = new Stack<int>(100);
        route.Push(EndPoint);
        Vertice current = _graph[EndPoint];
        while (current.PreviousVerticeInRouteIndex != -1)
        {
            route.Push(current.PreviousVerticeInRouteIndex);
            current = _graph.Vertices[current.PreviousVerticeInRouteIndex];
        }

        return route;
    }
    
    protected List<(Vertice vertice, float newDistance)> CalculateChildren(Vertice parent)
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