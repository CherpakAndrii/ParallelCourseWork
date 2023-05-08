namespace Model.Entities;

public partial class Graph
{
    private static float[][] BuildWeightMatrix(Model.Entities.Vertice[] vertices, bool[][] adjacensematrix)
    {
        int size = vertices.Length;
        float[][] weights = new float[size][];
        for (int i = 0; i < size; i++)
        {
            weights[i] = new float[size];
            for (int j = 0; j < size; j++)
            {
                weights[i][j] = adjacensematrix[i][j] ? GetDistance(vertices[i], vertices[j]) : -1;
            }
        }

        return weights;
    }
    
    private static float GetDistance(Model.Entities.Vertice v1, Model.Entities.Vertice v2) => (float)Math.Sqrt(Math.Pow(v1.VerticeCoordinates.X - v2.VerticeCoordinates.X, 2) +
                                                                           Math.Pow(v1.VerticeCoordinates.Y - v2.VerticeCoordinates.Y, 2));
}