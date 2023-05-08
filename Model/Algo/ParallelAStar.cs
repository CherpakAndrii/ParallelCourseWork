using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ParallelAStar : IPathSearchingAlgo
{
    public bool PathFound { get; set; }
    public byte NumberOfThreads { get; }

    public ParallelAStar(Graph graph, int startpoinIndex, int finishIndex, byte threads=12) : base(graph, startpoinIndex, finishIndex)
    {
        PathFound = false;
        NumberOfThreads = threads;
    }

    public override bool SearchPath()
    {
        BlockingPriorityQueue<int> verticeQueue = new BlockingPriorityQueue<int>();
        verticeQueue.Enqueue(StartPoint, 0);
        Vertice currentVertice;
        while (verticeQueue.Count > 0)
        {
            currentVertice = _graph[verticeQueue.Dequeue()];
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return true;

            foreach (int adjIndex in _graph.GetAdjacentVertices(currentVertice.OwnIndex))
            {
                Vertice child = _graph[adjIndex];
                if (child.TryUpdateMinRoute(currentVertice.OwnIndex))
                {
                    verticeQueue.Enqueue(adjIndex, child.DistanceFromStart+child.Heuristic!.Value);
                }
            }
        }

        return false;
    }
}