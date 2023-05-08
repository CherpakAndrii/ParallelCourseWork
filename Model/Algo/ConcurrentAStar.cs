using Model.Entities;

namespace Model.Algo;

public class ConcurrentAStar : IPathSearchingAlgo
{
    public ConcurrentAStar(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override bool SearchPath()
    {
        PriorityQueue<int, float> verticeQueue = new PriorityQueue<int, float>();
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

        return true;
    }
}