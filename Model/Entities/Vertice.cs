namespace ParallelAStar.model;

public partial class Vertice
{
    public double DistanceFromStart { get; set; }
    public Coordinates VerticeCoordinates { get; }
    public double Heuristic { get; private set; }
    public int PreviousVerticeInRouteIndex { get; set; }
    public bool IsPassed { get; set; }
    
    private int _ownIndex;
    private Graph _graph;

    public Vertice(Graph graph, int ownIndex, (int x, int y) coordinates)
    {
        VerticeCoordinates = new Coordinates(coordinates);
        DistanceFromStart = int.MaxValue/2;
        PreviousVerticeInRouteIndex = -1;
        IsPassed = false;
        _ownIndex = ownIndex;
        _graph = graph;
    }

    public bool TryUpdateMinRoute(int fromVerticeIndex)
    {
        if (IsPassed || _graph[fromVerticeIndex, _ownIndex] == -1)
            return false;

        double newDistance = _graph[fromVerticeIndex, _ownIndex] + _graph[fromVerticeIndex].DistanceFromStart;
        if (DistanceFromStart > newDistance)
        {
            DistanceFromStart = newDistance;
            PreviousVerticeInRouteIndex = fromVerticeIndex;
            return true;
        }

        return false;
    }

    public void SetHeuristic(Vertice finish)
    {
        Heuristic = Math.Sqrt(Math.Pow(VerticeCoordinates.X - finish.VerticeCoordinates.X, 2) +
                              Math.Pow(VerticeCoordinates.Y - finish.VerticeCoordinates.Y, 2));
    }
}