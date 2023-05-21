using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ParallelAStarOnTaskQueueOptimized : IPathSearchingAlgo
{
    public ParallelAStarOnTaskQueueOptimized(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override async Task<bool> SearchPath()
    {
        PriorityQueue<(Vertice, Task<List<(Vertice, float)>>?)> verticeQueue = new PriorityQueue<(Vertice, Task<List<(Vertice, float)>>?)>();
        Vertice currentVertice = _graph[StartPoint];
        Task<List<(Vertice, float)>>? calculateChildrenTask = Task.Run( () => CalculateChildren(currentVertice));
        verticeQueue.Enqueue((currentVertice, calculateChildrenTask), 0);
        while (verticeQueue.Count > 0)
        {
            (currentVertice, calculateChildrenTask) = verticeQueue.Dequeue();
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return true;

            var children = calculateChildrenTask is not null? await calculateChildrenTask : CalculateChildren(currentVertice);
            var bestIndex = children.MinBy(p => p.Item1.Heuristic + p.Item2).Item1.OwnIndex;
            foreach(var indDistPair in children)
            {
                Vertice child = indDistPair.Item1;
                if (!child.IsPassed && child.DistanceFromStart > indDistPair.Item2)
                {
                    child.PreviousVerticeInRouteIndex = currentVertice.OwnIndex;
                    child.DistanceFromStart = indDistPair.Item2;
                    Task<List<(Vertice, float)>>? calculateNextChildrenTask =
                        child.OwnIndex == bestIndex?
                        Task.Run(() => CalculateChildren(child)) : null;
                    verticeQueue.Enqueue((child, calculateNextChildrenTask),
                            child.DistanceFromStart + child.Heuristic!.Value);
                }
            }
        }

        return false;
    }
}