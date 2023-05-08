using System.Text;

namespace Model.Entities;

public partial class Graph
{
    public void SaveToFile(string filePath)
    {
        StringBuilder fileContent = new StringBuilder();
        foreach (var row in WeightMatrix)
        {
            foreach (var element in row)
            {
                fileContent.Append(element == -1 ? "0," : "1,");
            }

            fileContent.Append("\n");
        }

        fileContent.Append("////\n");

        foreach (var v in Vertices)
        {
            fileContent.Append($"{v.VerticeCoordinates.X}:{v.VerticeCoordinates.Y},");
        }
        
        File.WriteAllText(filePath, fileContent.ToString());
    }

    public static (bool[][], (int, int)[]) ReadFromFile(string filePath)
    {
        string[] content = File.ReadAllText(filePath).Split("////\n");
        string[] matrixLines = content[0].Split('\n', StringSplitOptions.RemoveEmptyEntries);
        string[] verticeCoordinates = content[1].Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (verticeCoordinates.Length != matrixLines.Length || matrixLines.Length != matrixLines[0].Length / 2)
            throw new ArgumentException("Matrix must be square, and it's size must be equal to vertices count");

        int size = matrixLines.Length;
        bool[][] adjacenseMatrix = new bool[size][];
        for (int i = 0; i < size; i++)
        {
            adjacenseMatrix[i] = new bool[size];
            for (int j = 0; j < size; j++)
            {
                adjacenseMatrix[i][j] = matrixLines[i][j * 2] == '1';
            }
        }

        (int, int)[] verticesCoordinates = new (int, int)[size];
        for (int i = 0; i < size; i++)
        {
            var coors = verticeCoordinates[i].Split(':');
            verticesCoordinates[i] = (int.Parse(coors[0]), int.Parse(coors[1]));
        }

        return (adjacenseMatrix, verticesCoordinates);
    }
}