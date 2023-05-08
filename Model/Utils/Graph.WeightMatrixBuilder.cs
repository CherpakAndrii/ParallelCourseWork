namespace Model.Entities;

public partial class Graph
{
    private static float[][] BuildWeightMatrix(Vertice[] vertices, bool[][] adjacensematrix)
    {
        int size = vertices.Length;
        float[][] weights = new float[size][];
        Parallel.For(0, size, i =>
        {
            weights[i] = new float[size];
            for (int j = 0; j < size; j++)
            {
                weights[i][j] = adjacensematrix[i][j] ? GetDistance(vertices[i], vertices[j]) : -1;
            }
        });

        return weights;
    }
    
    private static float GetDistance(Vertice v1, Vertice v2) => (float)Math.Sqrt(Math.Pow(v1.VerticeCoordinates.X - v2.VerticeCoordinates.X, 2) +
                                                                           Math.Pow(v1.VerticeCoordinates.Y - v2.VerticeCoordinates.Y, 2));
}