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

    public override float GetDistance(IVertice lastVertice)
    {
        return ((BilateralVertice)lastVertice).DistanceFromStart.Sum();
    }
}