using Model.Entities;

namespace Model.Algo;

public abstract class IPathSearchingAlgo
{
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
    }

    public abstract bool SearchPath();

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
}