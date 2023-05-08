using System.Diagnostics;
using Model.Algo;
using Model.Entities;

namespace ParallelAStar;

public static class Program
{
    public static void Main()
    {
        Graph g = new Graph(10);
        IPathSearchingAlgo algo = new ConcurrentAStar(g, 0, 9);
        
        Stopwatch sw_c = Stopwatch.StartNew();
        bool found = algo.SearchPath();
        sw_c.Stop();
        
        if (found)
        {
            var route = algo.TraceRoute();
        }
        
        g.Reset();
        algo = new Model.Algo.ParallelAStar(g, 0, 9);
        
        Stopwatch sw_p = Stopwatch.StartNew();
        found = algo.SearchPath();
        sw_p.Stop();
        
        if (found)
        {
            var route = algo.TraceRoute();
        }
    }
}