using Model.Entities;

namespace Model.Algo;

public abstract class ITwoSidePathSearchingAlgo: IPathSearchingAlgo
{
    protected ITwoSidePathSearchingAlgo(IGraph graph, int startpoinIndex, int finishIndex) : base(graph,
        startpoinIndex, finishIndex)
    {
        ((BilateralVertice)graph[startpoinIndex]).DistanceFromStart[0] = 0;
        ((BilateralVertice)graph[finishIndex]).DistanceFromStart[1] = 0;
    }
    public override Stack<int> TraceRoute(IVertice lastVertice)
    {
        Stack<int> routeToEnd = new Stack<int>(100);
        Stack<int> route = new Stack<int>(100);

        BilateralVertice current = (BilateralVertice)lastVertice;
        while (current.PreviousVerticeInRouteIndex[1] != -1)
        {
            routeToEnd.Push(current.PreviousVerticeInRouteIndex[1]);
            current = (BilateralVertice)_graph.Vertices[current.PreviousVerticeInRouteIndex[1]];
        }

        while (routeToEnd.Count > 0)
        {
            route.Push(routeToEnd.Pop());
        }
        
        route.Push(lastVertice.OwnIndex);
        current = (BilateralVertice)lastVertice;
        while (current.PreviousVerticeInRouteIndex[0] != -1)
        {
            route.Push(current.PreviousVerticeInRouteIndex[0]);
            current = (BilateralVertice)_graph.Vertices[current.PreviousVerticeInRouteIndex[0]];
        }

        return route;
    }
    
    protected List<(BilateralVertice vertice, float newDistance)> CalculateChildren(BilateralVertice parent, int processInd)
    {
        List<(BilateralVertice vertice, float newDistance)> updateDistances =
            new List<(BilateralVertice vertice, float newDistance)>();
        foreach (var adjIndex in _graph.GetAdjacentVertices(parent.OwnIndex))
        {
            BilateralVertice child = (BilateralVertice)_graph[adjIndex];
            Interlocked.Increment(ref ChildrenCalculatedCounter);
            if (child.IsPassed[processInd])
                continue;

            float newDistance = _graph[parent.OwnIndex, child.OwnIndex] + parent.DistanceFromStart[processInd];
            if (child.DistanceFromStart[processInd] > newDistance)
            {
                updateDistances.Add((child, newDistance));
            }
        }

        return updateDistances;
    }
    
    public override float GetDistance(IVertice lastVertice)
    {
        return ((BilateralVertice)lastVertice).DistanceFromStart.Sum();
    }
}