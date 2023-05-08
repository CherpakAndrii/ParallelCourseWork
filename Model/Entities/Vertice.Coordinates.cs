namespace ParallelAStar.model;

public partial class Vertice
{
    public class Coordinates
    {
        public int X { get; }
        public int Y { get; }

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinates((int x, int y) coors)
        {
            X = coors.x;
            Y = coors.y;
        }
    }
}