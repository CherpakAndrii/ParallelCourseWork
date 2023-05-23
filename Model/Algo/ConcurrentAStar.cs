using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ConcurrentAStar : ISingleSidePathSearchingAlgo
{
    public ConcurrentAStar(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex)
    {
        
    }

    public override async Task<IVertice?> SearchPath()
    {
        PriorityQueue<int> verticeQueue = new PriorityQueue<int>();
        Vertice currentVertice;
        verticeQueue.Enqueue(StartPoint, 0);
        while (verticeQueue.Count > 0)
        {
            currentVertice = (Vertice)_graph[verticeQueue.Dequeue()];
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return currentVertice;

            foreach (var adjIndex in _graph.GetAdjacentVertices(currentVertice.OwnIndex))
            {
                Vertice child = (Vertice)_graph[adjIndex];
                Interlocked.Increment(ref ChildrenCalculatedCounter);
                if (child.TryUpdateMinRoute(currentVertice.OwnIndex))
                    verticeQueue.Enqueue(child.OwnIndex, child.DistanceFromStart + child.Heuristic!.Value);
            }
        }

        return null;
    }
}
