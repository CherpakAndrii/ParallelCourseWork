using Model.Entities;

namespace Model.Algo;

public class ConcurrentAStar : IPathSearchingAlgo
{
    public ConcurrentAStar(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override bool SearchPath()
    {
        throw new NotImplementedException();
    }
}