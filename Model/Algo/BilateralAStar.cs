using Model.DataStructures;
using Model.Entities;

namespace Model.Algo;

public class BilateralAStar : ITwoSidePathSearchingAlgo
{
    private BilateralVertice? meet;
    public BilateralAStar(BilateralGraph graph, int startpoinIndex, int finishIndex) : base(graph, startpoinIndex, finishIndex) { }

    public override async Task<IVertice?> SearchPath()
    {
        List<Task<bool>> searchTasks = new List<Task<bool>>();
        searchTasks.Add(Task.Run(() => SingleSidePathSearching(0)));
        searchTasks.Add(Task.Run(() => SingleSidePathSearching(1)));

        await Task.WhenAny(searchTasks);
        return meet;
    }

    public async Task<bool> SingleSidePathSearching(int procInd)
    {
        PriorityQueue<int> verticeQueue = new PriorityQueue<int>();
        BilateralVertice currentVertice;
        verticeQueue.Enqueue(procInd == 0? StartPoint : EndPoint, 0);
        while (verticeQueue.Count > 0 && meet is null)
        {
            currentVertice = (BilateralVertice)_graph[verticeQueue.Dequeue()];
            if (currentVertice.IsPassed[procInd])
                continue;
            
            currentVertice.IsPassed[procInd] = true;
            if (currentVertice.IsPassed[(procInd+1)%2])
            {
                meet = currentVertice;
                return true;
            }

            foreach (var adjIndex in _graph.GetAdjacentVertices(currentVertice.OwnIndex))
            {
                BilateralVertice child = (BilateralVertice)_graph[adjIndex];
                Interlocked.Increment(ref ChildrenCalculatedCounter);
                if (child.TryUpdateMinRoute(currentVertice.OwnIndex, procInd))
                    verticeQueue.Enqueue(child.OwnIndex,
                        child.DistanceFromStart[procInd] + child.Heuristic[procInd].Value);
            }
        }

        return false;
    }
}
