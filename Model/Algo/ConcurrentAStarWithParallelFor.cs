using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ConcurrentAStarWithParallelFor : IPathSearchingAlgo
{
    public ConcurrentAStarWithParallelFor(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public async override Task<bool> SearchPath()
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

            await Task.Run(() =>
            {
                Parallel.ForEach(_graph.GetAdjacentVertices(currentVertice.OwnIndex), adjIndex =>
                {
                    Vertice child = _graph[adjIndex];
                    Interlocked.Increment(ref ChildrenCalculatedCounter);
                    if (child.TryUpdateMinRoute(currentVertice.OwnIndex))
                    {
                        verticeQueue.Enqueue(adjIndex, child.DistanceFromStart + child.Heuristic!.Value);
                    }
                });
            });
        }

        return false;
    }
}
