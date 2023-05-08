using System.Diagnostics;
using Model.Algo;
using Model.Entities;

namespace ConsoleApp;

public static class Program
{
    public static void Main()
    {
        Graph g = new Graph(10000);
        g.SaveToBinFile("saved.grph");
        int s = 0, f = 6789;
        IPathSearchingAlgo algo = new ConcurrentAStar(g, s, f);
        TestAlgo(algo, g, f);
        g.Reset();
        algo = new ParallelAStarOnWaitingTasks(g, s, f);
        TestAlgo(algo, g, f);
        g.Reset();
        algo = new ParallelAStarOnTaskQueue(g, s, f);
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
            Console.WriteLine($"found: {g[f].DistanceFromStart} ({route.Count}) in {sw.ElapsedMilliseconds}ms");
        }
        else
            Console.WriteLine($"Not found in {sw.ElapsedMilliseconds}ms");
    }
}