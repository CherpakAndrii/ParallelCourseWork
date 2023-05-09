using System.Diagnostics;
using Model.Algo;
using Model.Entities;

namespace ConsoleApp;

public static class Program
{
    public static async Task Main()
    {
        Graph g = new Graph("saved.grph", Graph.FileType.Binary);
        // g.SaveToBinFile("saved.grph");
        int s = 0, f = 6789;
        IPathSearchingAlgo algo = new ConcurrentAStar(g, s, f);
        await TestAlgo(algo, g, f);
        g.Reset();
        algo = new ParallelAStarOnWaitingTasks(g, s, f);
        await TestAlgo(algo, g, f);
        g.Reset();
        algo = new ParallelAStarOnTaskQueue(g, s, f);
        await TestAlgo(algo, g, f);
    }

    static async Task TestAlgo(IPathSearchingAlgo algo, Graph g, int f)
    {
        Stopwatch sw = Stopwatch.StartNew();
        bool found = await algo.SearchPath();
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