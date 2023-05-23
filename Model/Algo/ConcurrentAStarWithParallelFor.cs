using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ConcurrentAStarWithParallelFor : ISingleSidePathSearchingAlgo
{
    public ConcurrentAStarWithParallelFor(Graph graph, int startpoinIndex, int finishIndex) : base(graph,
        startpoinIndex, finishIndex) { }

    public async override Task<IVertice?> SearchPath()
    {
        BlockingPriorityQueue<int> verticeQueue = new BlockingPriorityQueue<int>();
        verticeQueue.Enqueue(StartPoint, 0);
        Vertice currentVertice;
        while (verticeQueue.Count > 0)
        {
            currentVertice = (Vertice)_graph[verticeQueue.Dequeue()];
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return currentVertice;

            await Task.Run(() =>
            {
                Parallel.ForEach(_graph.GetAdjacentVertices(currentVertice.OwnIndex), adjIndex =>
                {
                    Vertice child = (Vertice)_graph[adjIndex];
                    Interlocked.Increment(ref ChildrenCalculatedCounter);
                    if (child.TryUpdateMinRoute(currentVertice.OwnIndex))
                    {
                        verticeQueue.Enqueue(adjIndex, child.DistanceFromStart + child.Heuristic!.Value);
                    }
                });
            });
        }

        return null;
    }
}
