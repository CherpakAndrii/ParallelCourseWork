using System.Diagnostics;
using Model.Algo;
using Model.Entities;

namespace ParallelAStar;

public static class Program
{
    public static void Main()
    {
        Graph g = new Graph(10000);
        Console.WriteLine("Graph generated!");
        int s = 0, f = 1234;
        IPathSearchingAlgo algo = new ConcurrentAStar(g, s, f);
        TestAlgo(algo, g, f);
        g.Reset();
        algo = new Model.Algo.ParallelAStar(g, s, f);
        TestAlgo(algo, g, f);
    }

    static void TestAlgo(IPathSearchingAlgo algo, Graph g, int f)
    {
        Stopwatch sw = Stopwatch.StartNew();
        bool found = algo.SearchPath();
        sw.Stop();
        
        if (found)
        {
            var route = algo.TraceRoute();
            Console.WriteLine($"found: {g[f].DistanceFromStart} ({route.Count()}) in {sw.ElapsedMilliseconds}ms");
        }
        else
            Console.WriteLine("Not found");
    }
}