using System.Text;

namespace Model.Entities;

public partial class Graph
{
    public void SaveToTextFile(string filePath)
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
    
    public void SaveToBinFile(string filePath)
    {
        int graphSize = Vertices.Length;
        BinaryWriter fileContent = new BinaryWriter(new FileStream(filePath, FileMode.Create));
        fileContent.Write(graphSize);
        foreach (var row in WeightMatrix)
        {
            foreach (var element in row)
            {
                fileContent.Write((byte)(element == -1 ? 0 : 1));
            }
        }
        foreach (var v in Vertices)
        {
            fileContent.Write(v.VerticeCoordinates.X);
            fileContent.Write(v.VerticeCoordinates.Y);
        }
        
        fileContent.Close();
    }

    public static (bool[][], (int, int)[]) ReadFromTxtFile(string filePath)
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
    
    public static (bool[][], (int, int)[]) ReadFromBinFile(string filePath)
    {
        BinaryReader br = new BinaryReader(new FileStream(filePath, FileMode.Open));
        int graphSize = br.ReadInt32();

        bool[][] adjacenseMatrix = new bool[graphSize][];
        for (int i = 0; i < graphSize; i++)
        {
            adjacenseMatrix[i] = new bool[graphSize];
            for (int j = 0; j < graphSize; j++)
            {
                adjacenseMatrix[i][j] = br.ReadBoolean();
            }
        }

        (int, int)[] verticesCoordinates = new (int, int)[graphSize];
        for (int i = 0; i < graphSize; i++)
        {
            verticesCoordinates[i] = (br.ReadInt32(), br.ReadInt32());
        }

        return (adjacenseMatrix, verticesCoordinates);
    }
}