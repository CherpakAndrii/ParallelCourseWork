using System.Collections.Concurrent;
using Model.DataStructures;
using Model.Entities;

namespace Model.Algo
{
    public class HDAStar : ISingleSidePathSearchingAlgo
    {
        private readonly int _numProcessors; // Number of processors to distribute the workload
        private readonly ConcurrentDictionary<int, BlockingPriorityQueue<int>> _processorQueues;
        private readonly object _lock = new object();

        public HDAStar(Graph graph, int startpoinIndex, int finishIndex, int numProcessors) : base(graph, startpoinIndex, finishIndex)
        {
            _numProcessors = numProcessors;
            _processorQueues = new ConcurrentDictionary<int, BlockingPriorityQueue<int>>();
            for (int i = 0; i < _numProcessors; i++)
            {
                _processorQueues[i] = new BlockingPriorityQueue<int>();
            }
        }

        public async override Task<IVertice> SearchPath()
        {
            _processorQueues[0].Enqueue(StartPoint, 0);
            bool foundPath = false;

            while (_processorQueues.Values.Any(q => q.Count != 0))
            {
                await Task.Run(() =>
                {
                    Parallel.ForEach(_processorQueues.Keys, processorId =>
                    {
                        if (_processorQueues[processorId].Count != 0 && !foundPath)
                        {
                            Vertice currentVertice = (Vertice)_graph[_processorQueues[processorId].Dequeue()];
                            if (!currentVertice.IsPassed)
                            {
                                lock (_lock)
                                {
                                    currentVertice.IsPassed = true;
                                }

                                if (currentVertice.OwnIndex == EndPoint)
                                {
                                    foundPath = true;
                                }

                                foreach (int adjIndex in _graph.GetAdjacentVertices(currentVertice.OwnIndex))
                                {
                                    Vertice child = (Vertice)_graph[adjIndex];
                                    bool shouldEnqueue = false;
                                    lock (child)
                                    {
                                        if (!child.IsPassed)
                                        {
                                            shouldEnqueue = child.TryUpdateMinRoute(currentVertice.OwnIndex);
                                        }
                                    }
                                    if (shouldEnqueue)
                                    {
                                        int hash = ComputeHash(adjIndex);
                                        _processorQueues[hash].Enqueue(adjIndex, child.DistanceFromStart + child.Heuristic!.Value);
                                    }
                                }
                            }
                        }
                    });
                });

                if (foundPath)
                    break;
            }

            return foundPath? (Vertice)_graph[EndPoint] : null;
        }

        private int ComputeHash(int vertexIndex)
        {
            // Custom hash function to distribute vertices across processors
            return vertexIndex % _numProcessors;
        }
    }
}
