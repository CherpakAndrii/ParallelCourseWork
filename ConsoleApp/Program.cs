using System.Diagnostics;
using Model.Algo;
using Model.Entities;

namespace ConsoleApp;

public static class Program
{
    public static async Task Main()
    {
        Graph g = new Graph("saved.grph", IGraph.FileType.Binary);//new Graph(50000);
        // IGraph.SaveToBinFile("saved.grph", g);
        BilateralGraph bg = new BilateralGraph("saved.grph", IGraph.FileType.Binary);

        Console.WriteLine("Building graphs done\n\n-----------------------------Testing:-------------------------");
        
        int s = 0, f = 6789;
        IPathSearchingAlgo algo = new ConcurrentAStar(g, s, f);
        await TestAlgo(algo, "Concurrent A*", true);
        g.Reset();
        algo = new ConcurrentAStarWithParallelFor(g, s, f);
        await TestAlgo(algo, "A* with ParallelFor");
        g.Reset();
        algo = new ParallelAStarOnWaitingTasks(g, s, f);
        await TestAlgo(algo, "Parallel A* on waiting tasks");
        g.Reset();
        algo = new ParallelAStarOnTaskQueue(g, s, f);
        await TestAlgo(algo, "Parallel A* with task queueing");
        g.Reset();
        algo = new BilateralAStar(bg, s, f);
        await TestAlgo(algo, "Bilateral A*");
    }

    public static async Task TestAlgo(IPathSearchingAlgo algo, string algoName, bool printAllPath = false)
    {
        Stopwatch sw = Stopwatch.StartNew();
        IVertice? found = await algo.SearchPath();
        sw.Stop();
        
        if (found is not null)
        {
            var route = algo.TraceRoute(found);
            Console.WriteLine($"Path found by {algoName}:\n\t{algo.GetDistance(found)} (through {route.Count} vertices) in {sw.ElapsedMilliseconds}ms with {algo.ChildrenCalculatedCounter} vertices touched for graph {algo.GetGraphSize()}x{algo.GetGraphSize()}");
            if (printAllPath)
            {
                Console.Write('\t');
                Console.Write(route.Pop());
                while (route.Count > 0)
                {
                    Console.Write(" --> "+route.Pop());
                }

                Console.WriteLine('\n');
            }
        }
        else
            Console.WriteLine($"Path not found by {algoName} in {sw.ElapsedMilliseconds}ms with {algo.ChildrenCalculatedCounter} vertices touched for graph {algo.GetGraphSize()}x{algo.GetGraphSize()}");
    }
}