using Model.Entities;

namespace Model.Algo;

public abstract class ISingleSidePathSearchingAlgo: IPathSearchingAlgo
{
    protected ISingleSidePathSearchingAlgo(Graph graph, int startpoinIndex, int finishIndex) : base(graph,
        startpoinIndex, finishIndex)
    {
        ((Vertice)graph[startpoinIndex]).DistanceFromStart = 0;
    }
    public override Stack<int> TraceRoute(IVertice lastVertice)
    {
        Stack<int> route = new Stack<int>(100);
        route.Push(EndPoint);
        Vertice current = (Vertice)_graph[EndPoint];
        while (current.PreviousVerticeInRouteIndex != -1)
        {
            route.Push(current.PreviousVerticeInRouteIndex);
            current = (Vertice)_graph.Vertices[current.PreviousVerticeInRouteIndex];
        }

        return route;
    }
    
    protected List<(Vertice vertice, float newDistance)> CalculateChildren(Vertice parent)
    {
        List<(Vertice vertice, float newDistance)> updateDistances =
            new List<(Vertice vertice, float newDistance)>();
        foreach (var adjIndex in _graph.GetAdjacentVertices(parent.OwnIndex))
        {
            Vertice child = (Vertice)_graph[adjIndex];
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

    public override float GetDistance(IVertice lastVertice)
    {
        return ((Vertice)lastVertice).DistanceFromStart;
    }
}