namespace Model.Entities;

public class BilateralVertice: IVertice
{
    public float[] DistanceFromStart { get; set; }
    public Coordinates VerticeCoordinates { get; }
    public int OwnIndex { get; }
    public float?[] Heuristic { get; private set; }
    public int[] PreviousVerticeInRouteIndex { get; set; }
    public bool[] IsPassed;
    private IGraph _graph;

    public BilateralVertice(IGraph graph, int ownIndex, (int x, int y) coordinates)
    {
        VerticeCoordinates = new Coordinates(coordinates);
        DistanceFromStart = new [] { float.MaxValue / 2, float.MaxValue / 2 };
        PreviousVerticeInRouteIndex = new[] { -1, -1 };
        IsPassed = new[] { false, false };
        OwnIndex = ownIndex;
        _graph = graph;
    }

    public IVertice Copy()
    {
        return new BilateralVertice(_graph, OwnIndex, (VerticeCoordinates.X, VerticeCoordinates.Y));
    }

    public bool TryUpdateMinRoute(int fromVerticeIndex, int processIndex)
    {
        if (IsPassed[processIndex] || _graph[fromVerticeIndex, OwnIndex] == -1)
            return false;

        float newDistance = _graph[fromVerticeIndex, OwnIndex] + ((BilateralVertice)_graph[fromVerticeIndex]).DistanceFromStart[processIndex];
        lock (this)
        {
            if (DistanceFromStart[processIndex] > newDistance)
            {
                DistanceFromStart[processIndex] = newDistance;
                PreviousVerticeInRouteIndex[processIndex] = fromVerticeIndex;
                return true;
            }
        }
        return false;
    }

    public void SetHeuristic(IVertice start, IVertice finish)
    {
        Heuristic = new float?[2];
        Heuristic[0] = (float)Math.Sqrt(Math.Pow(VerticeCoordinates.X - finish.VerticeCoordinates.X, 2) +
                                     Math.Pow(VerticeCoordinates.Y - finish.VerticeCoordinates.Y, 2));
        Heuristic[1] = (float)Math.Sqrt(Math.Pow(VerticeCoordinates.X - start.VerticeCoordinates.X, 2) +
                                        Math.Pow(VerticeCoordinates.Y - start.VerticeCoordinates.Y, 2));
    }

    public void Reset()
    {
        DistanceFromStart = new[] { float.MaxValue / 2, float.MaxValue / 2 };
        PreviousVerticeInRouteIndex = new[] { -1, -1 };
        Heuristic = new float?[] { null, null };
        IsPassed = new [] { false, false };
    }
}