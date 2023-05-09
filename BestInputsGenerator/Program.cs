using Model.DataStructures;
using Model.Entities;

namespace BestInputsGenerator;

public static class Program
{
    public static void Main()
    {
        Graph g = new Graph(50000);
        g.SaveToBinFile("saved.grph");
        Console.WriteLine("Graph saved to file");
        
        List<Task<(int startInd, (int finInd, float dist) searchRes)>> tasks = new();
        for (int i = 1; i < g.Vertices.Length; i++)
        {
            tasks.Add(Task.Run(() => (i, GetTheMostFarPoint(new Graph(g), i))));
            Console.Write("\r"+i);
        }

        Task.WaitAll(tasks.ToArray());
        var mostFar = tasks.Select(t => t.Result).MaxBy(p => p.searchRes.dist);

        Console.WriteLine($"{mostFar.startInd} --> {mostFar.searchRes.finInd} : {mostFar.searchRes.dist}");
    }

    static (int ind, float dist) GetTheMostFarPoint(Graph graph, int start)
    {
        PriorityQueue<int> verticeQueue = new PriorityQueue<int>();
        verticeQueue.Enqueue(start, 0);
        Vertice currentVertice = graph[start];
        currentVertice.DistanceFromStart = 0;
        while (verticeQueue.Count > 0)
        {
            currentVertice = graph[verticeQueue.Dequeue()];
            if (currentVertice.IsPassed)
                continue;
            
            currentVertice.IsPassed = true;
            foreach (int adjIndex in graph.GetAdjacentVertices(currentVertice.OwnIndex, start))
            {
                Vertice child = graph[adjIndex];
                if (child.TryUpdateMinRoute(currentVertice.OwnIndex))
                {
                    verticeQueue.Enqueue(adjIndex, child.DistanceFromStart);
                }
            }
        }

        return (currentVertice.OwnIndex, currentVertice.DistanceFromStart);
    }
}