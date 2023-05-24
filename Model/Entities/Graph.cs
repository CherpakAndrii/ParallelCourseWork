namespace Model.Entities;

public class Graph : IGraph
{
    public float[][] WeightMatrix { get; set; }
    public IVertice[] Vertices { get; }
    public int StartVerticeIndex { get; set; }
    public int FinishVerticeIndex { get; set; }

    public Graph(int size)
    {
        if (size < 3)
            throw new ArgumentException("The graph must contain at least 3 vertices!");

        Vertices = GraphGenerator.GenerateVertices(size, this);
        Console.WriteLine("Vertices generated!");
        bool[][] _adjMatrix = GraphGenerator.GenerateAdjacenceMatrix(size, size < 1000? 0.3f : 0.01f);
        Console.WriteLine("Adj matrix generated!");
        WeightMatrix = IGraph.BuildWeightMatrix(Vertices, _adjMatrix);
        Console.WriteLine("Weight matrix built!");
    }

    public void Reset()
    {
        Parallel.ForEach(Vertices, v => v.Reset());
    }

    public Graph(string sourceFilePath, IGraph.FileType fileType)
    {
        (bool[][] adjMatrix, (int, int)[] coordinates) = fileType == IGraph.FileType.Text
            ? IGraph.ReadFromTxtFile(sourceFilePath)
            : IGraph.ReadFromBinFile(sourceFilePath);
        Console.WriteLine("File reading done");
        Vertices = new Vertice[coordinates.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            Vertices[i] = new Vertice(this, i, coordinates[i]);
        }
        WeightMatrix = IGraph.BuildWeightMatrix(Vertices, adjMatrix);
    }

    public IVertice this[int ind]
    {
        get => Vertices[ind];
    }

    public float this[int vertice1, int vertice2]
    {
        get
        {
            if (!IndexIsInRange(vertice1) || !IndexIsInRange(vertice2))
                throw new IndexOutOfRangeException("Vertice index is out of matrix");
            return WeightMatrix[vertice1][vertice2];
        }
    }

    public Graph(IGraph original)
    {
        WeightMatrix = original.WeightMatrix;
        Vertices = new IVertice[original.Vertices.Length];
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] = original.Vertices[i].Copy();
        }
    }

    public void SetStartEndPoint(int startPoint, int endPointIndex)
    {
        FinishVerticeIndex = endPointIndex;
        IVertice finish = Vertices[endPointIndex];
        IVertice start = Vertices[startPoint];
        Parallel.ForEach(Vertices, vertice => vertice.SetHeuristic(start, finish));
    }

    private bool IndexIsInRange(int index) => index >= 0 && index < WeightMatrix.Length;
}