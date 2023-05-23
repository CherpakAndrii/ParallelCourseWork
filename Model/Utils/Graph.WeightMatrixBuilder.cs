namespace Model.Entities;

public partial interface IGraph
{
    protected static float[][] BuildWeightMatrix(IVertice[] vertices, bool[][] adjacensematrix)
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
    
    private static float GetDistance(IVertice v1, IVertice v2) => (float)Math.Sqrt(Math.Pow(v1.VerticeCoordinates.X - v2.VerticeCoordinates.X, 2) +
                                                                           Math.Pow(v1.VerticeCoordinates.Y - v2.VerticeCoordinates.Y, 2));
}