using Model.Entities;

namespace Model.Algo;

public abstract class IPathSearchingAlgo
{
    public int ChildrenCalculatedCounter;
    protected int StartPoint { get; }
    protected int EndPoint { get; }
    protected IGraph _graph { get; }
    protected IPathSearchingAlgo(IGraph graph, int startpoinIndex, int finishIndex)
    {
        StartPoint = startpoinIndex;
        EndPoint = finishIndex;
        graph.SetStartEndPoint(startpoinIndex, finishIndex);
        _graph = graph;
        ChildrenCalculatedCounter = 0;
    }

    public abstract Task<IVertice?> SearchPath();

    public abstract Stack<int> TraceRoute(IVertice lastVertice);
    public abstract float GetDistance(IVertice lastVertice);
    public int GetGraphSize() => _graph.Vertices.Length;
}