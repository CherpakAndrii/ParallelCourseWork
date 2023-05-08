namespace ParallelAStar.model;

public partial class Graph
{
    private static double[][] BuildWeightMatrix(Vertice[] vertices, bool[][] adjacensematrix)
    {
        int size = vertices.Length;
        double[][] weights = new double[size][];
        for (int i = 0; i < size; i++)
        {
            weights[i] = new double[size];
            for (int j = 0; j < size; j++)
            {
                weights[i][j] = adjacensematrix[i][j] ? GetDistance(vertices[i], vertices[j]) : -1;
            }
        }

        return weights;
    }
    
    private static double GetDistance(Vertice v1, Vertice v2) => Math.Sqrt(Math.Pow(v1.VerticeCoordinates.X - v2.VerticeCoordinates.X, 2) +
                                                                           Math.Pow(v1.VerticeCoordinates.Y - v2.VerticeCoordinates.Y, 2));
}