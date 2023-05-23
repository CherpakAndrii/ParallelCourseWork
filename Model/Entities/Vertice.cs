namespace Model.Entities;

public class Vertice : IVertice
{
    public float DistanceFromStart { get; set; }
    public Coordinates VerticeCoordinates { get; }
    public float? Heuristic { get; private set; }
    public int PreviousVerticeInRouteIndex { get; set; }
    public bool IsPassed;
    
    public int OwnIndex { get; }
    private IGraph Graph { get; }

    public Vertice(IGraph graph, int ownIndex, (int x, int y) coordinates)
    {
        VerticeCoordinates = new Coordinates(coordinates);
        DistanceFromStart = float.MaxValue/2;
        PreviousVerticeInRouteIndex = -1;
        IsPassed = false;
        OwnIndex = ownIndex;
        Graph = graph;
    }

    public IVertice Copy()
    {
        return new Vertice(Graph, OwnIndex, (VerticeCoordinates.X, VerticeCoordinates.Y));
    }

    public void SetHeuristic(IVertice start, IVertice finish)
    {
        Heuristic = (float)Math.Sqrt(Math.Pow(VerticeCoordinates.X - finish.VerticeCoordinates.X, 2) +
                                     Math.Pow(VerticeCoordinates.Y - finish.VerticeCoordinates.Y, 2));    }

    public bool TryUpdateMinRoute(int fromVerticeIndex)
    {
        if (IsPassed || Graph[fromVerticeIndex, OwnIndex] == -1)
            return false;

        float newDistance = Graph[fromVerticeIndex, OwnIndex] + ((Vertice)Graph[fromVerticeIndex]).DistanceFromStart;
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

    public void Reset()
    {
        DistanceFromStart = float.MaxValue / 2;
        PreviousVerticeInRouteIndex = -1;
        Heuristic = null;
        IsPassed = false;
    }
}