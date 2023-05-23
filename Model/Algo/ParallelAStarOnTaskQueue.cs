using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class ParallelAStarOnTaskQueue : ISingleSidePathSearchingAlgo
{
    public ParallelAStarOnTaskQueue(Graph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override async Task<IVertice?> SearchPath()
    {
        PriorityQueue<(Vertice, Task<List<(Vertice, float)>>?)> verticeQueue = new PriorityQueue<(Vertice, Task<List<(Vertice, float)>>?)>();
        Vertice currentVertice = (Vertice)_graph[StartPoint];
        Task<List<(Vertice, float)>>? calculateChildrenTask = Task.Run( () => CalculateChildren(currentVertice));
        verticeQueue.Enqueue((currentVertice, calculateChildrenTask), 0);
        while (verticeQueue.Count > 0)
        {
            (currentVertice, calculateChildrenTask) = verticeQueue.Dequeue();
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            if (currentVertice.OwnIndex == EndPoint)
                return currentVertice;

            var children = (calculateChildrenTask is not null? await calculateChildrenTask : CalculateChildren(currentVertice));
            var minPriorPair = children.MinBy(p => p.Item1.Heuristic!.Value + p.Item2);
            float minPrior = minPriorPair.Item1.Heuristic!.Value + minPriorPair.Item2;
            foreach (var pair in children)
            {
                Vertice child = pair.Item1;
                if (!child.IsPassed && child.DistanceFromStart > pair.Item2)
                {
                    child.PreviousVerticeInRouteIndex = currentVertice.OwnIndex;
                    child.DistanceFromStart = pair.Item2;
                    float prior = child.Heuristic!.Value + pair.Item2;
                    Task<List<(Vertice, float)>>? calculateNextChildrenTask =
                        prior <= minPrior+1 ? Task.Run(() => CalculateChildren(child)) : null;
                    verticeQueue.Enqueue((child, calculateNextChildrenTask), prior);
                }
            }
        }

        return null;
    }
}