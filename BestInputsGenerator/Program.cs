using Model.DataStructures;
using Model.Entities;

namespace BestInputsGenerator;

public static class Program
{
    public static void Main()
    {
        Graph g = new Graph(40000);
        g.SaveToBinFile("saved.grph");
        Console.WriteLine("Graph saved to file");
        float currentMaxDist = 0;
        (int, int) currentMostFarIndexes = (-1, -1);
        (int ind, float dist) currentMostFar;
        for (int i = 1; i < g.Vertices.Length; i++)
        {
            currentMostFar = GetTheMostFarPoint(new Graph(g), i);
            if (currentMostFar.dist > currentMaxDist)
            {
                currentMaxDist = currentMostFar.dist;
                currentMostFarIndexes = (i, currentMostFar.ind);
            }
            g.Reset();
            Console.Write("\r"+i);
        }

        Console.WriteLine($"{currentMostFarIndexes.Item1} --> {currentMostFarIndexes.Item2} : {currentMaxDist}");
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