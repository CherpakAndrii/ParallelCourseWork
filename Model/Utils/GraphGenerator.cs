using ParallelAStar.model;

namespace ParallelAStar.utils;

public static class GraphGenerator
{
    public static bool[][] GenerateAdjacenceMatrix(int verticeNumber, float adjacenceCoefficient = 0.3f)
    {
        if (verticeNumber <= 2 || adjacenceCoefficient is <= 0 or >= 1)
            throw new ArgumentException(
                "Incorrect arguments! Make sure that vertice number is greater than 2 and adjacence coefficient is between 0 and 1.");

        Random rand = new Random();
        const int accuracy = 1000;
        float acceptanceBoundary = accuracy * adjacenceCoefficient;

        bool[][] matrix = new bool[verticeNumber][];
        for (int i = 0; i < verticeNumber; i++)
        {
            matrix[i] = new bool[verticeNumber];
            for (int j = 0; j < i; j++)
                matrix[i][j] = matrix[j][i] = rand.Next(accuracy) <= acceptanceBoundary;
        }
        
        return matrix;
    }

    public static Vertice[] GenerateVertices(int verticeNumber, Graph graph)
    {
        Vertice[] vertices = new Vertice[verticeNumber];
        Random random = new Random();
        int upperBoundOfCoordinates = verticeNumber * 3;
        for (int i = 0; i < verticeNumber; i++)
        {
            vertices[i] = new Vertice(graph, i,
                (random.Next(upperBoundOfCoordinates), random.Next(upperBoundOfCoordinates)));
        }

        return vertices;
    }
}