using System.Diagnostics;
using Model.Algo;
using Model.Entities;

namespace ConsoleApp;

public static class Program
{
    public static async Task Main()
    {
        Graph g = new Graph("saved60.grph", IGraph.FileType.Binary);//*/new Graph(60000);
        //IGraph.SaveToBinFile("saved60.grph", g);
        BilateralGraph bg = new BilateralGraph("saved60.grph", IGraph.FileType.Binary);
        
        Console.WriteLine("Building graphs done\n\n-----------------------------Testing:-------------------------");
        
        int s = 12345, f = 56789;
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

        //await TestResults();
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
                PrintPath(route);
            }
        }
        else
            Console.WriteLine($"Path not found by {algoName} in {sw.ElapsedMilliseconds}ms with {algo.ChildrenCalculatedCounter} vertices touched for graph {algo.GetGraphSize()}x{algo.GetGraphSize()}");
    }

    private static void PrintPath(Stack<int> route)
    {
        Console.Write('\t');
        Console.Write(route.Pop());
        while (route.Count > 0)
        {
            Console.Write(" --> "+route.Pop());
        }

        Console.WriteLine('\n');
    }

    public static async Task TestResults()
    {
        Graph g = new Graph(10);
        PrintGraph(g);
        
        IPathSearchingAlgo algo = new ConcurrentAStar(g, 1, 4);
        var last = await algo.SearchPath();
        if (last is  null)
        {
            Console.WriteLine("Path is not found!");
            return;
        }
        
        var route = algo.TraceRoute(last);
        Console.WriteLine("Path found:");
        PrintPath(route);
    }

    private static void PrintGraph(IGraph graph)
    {
        Console.WriteLine("\tМатриця вагiв:");
        foreach (var row in graph.WeightMatrix)
        {
            foreach (var element in row)
            {
                Console.Write((element > -1 ? Math.Round(element, 2).ToString() : "-") + "\t");
            }

            Console.WriteLine();
        }

        Console.WriteLine("\n");
    }
}
