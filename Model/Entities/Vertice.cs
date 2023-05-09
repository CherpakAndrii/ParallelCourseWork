namespace Model.Entities;

public partial class Vertice
{
    public float DistanceFromStart { get; set; }
    public Coordinates VerticeCoordinates { get; }
    public float? Heuristic { get; private set; }
    public int PreviousVerticeInRouteIndex { get; set; }
    public bool IsPassed;
    
    public int OwnIndex;
    private Graph _graph;

    public Vertice(Graph graph, int ownIndex, (int x, int y) coordinates)
    {
        VerticeCoordinates = new Coordinates(coordinates);
        DistanceFromStart = float.MaxValue/2;
        PreviousVerticeInRouteIndex = -1;
        IsPassed = false;
        OwnIndex = ownIndex;
        _graph = graph;
    }

    public Vertice Copy()
    {
        return new Vertice(_graph, OwnIndex, (VerticeCoordinates.X, VerticeCoordinates.Y));
    }

    public bool TryUpdateMinRoute(int fromVerticeIndex)
    {
        if (IsPassed || _graph[fromVerticeIndex, OwnIndex] == -1)
            return false;

        float newDistance = _graph[fromVerticeIndex, OwnIndex] + _graph[fromVerticeIndex].DistanceFromStart;
        lock (this)
        {
            if (DistanceFromStart > newDistance)
            {
                DistanceFromStart = newDistance;
                PreviousVerticeInRouteIndex = fromVerticeIndex;
                return true;
            }
        }
        return false;
    }

    public void SetHeuristic(Vertice finish)
    {
        Heuristic = (float)Math.Sqrt(Math.Pow(VerticeCoordinates.X - finish.VerticeCoordinates.X, 2) +
                              Math.Pow(VerticeCoordinates.Y - finish.VerticeCoordinates.Y, 2));
    }

    public void Reset()
    {
        DistanceFromStart = float.MaxValue / 2;
        PreviousVerticeInRouteIndex = -1;
        Heuristic = null;
        IsPassed = false;
    }
}