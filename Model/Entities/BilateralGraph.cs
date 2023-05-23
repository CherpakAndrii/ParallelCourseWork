namespace Model.Entities;

public class BilateralGraph : IGraph
{
    public float[][] WeightMatrix { get; set; }
    public IVertice[] Vertices { get; }
    public int StartVerticeIndex { get; set; }
    public int FinishVerticeIndex { get; set; }

    public BilateralGraph(int size)
    {
        if (size < 3)
            throw new ArgumentException("The graph must contain at least 3 vertices!");

        Vertices = GraphGenerator.GenerateVertices(size, this);
        Console.WriteLine("Vertices generated!");
        bool[][] _adjMatrix = GraphGenerator.GenerateAdjacenceMatrix(size, 0.3f);
        Console.WriteLine("Adj matrix generated!");
        WeightMatrix = IGraph.BuildWeightMatrix(Vertices, _adjMatrix);
        Console.WriteLine("Weight matrix built!");
    }

    public void Reset()
    {
        Parallel.ForEach(Vertices, v => v.Reset());
    }

    public BilateralGraph(string sourceFilePath, IGraph.FileType fileType)
    {
        (bool[][] adjMatrix, (int, int)[] coordinates) = fileType == IGraph.FileType.Text
            ? IGraph.ReadFromTxtFile(sourceFilePath)
            : IGraph.ReadFromBinFile(sourceFilePath);
        Console.WriteLine("File reading done");
        Vertices = new BilateralVertice?[coordinates.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            Vertices[i] = new BilateralVertice(this, i, coordinates[i]);
        }
        WeightMatrix = IGraph.BuildWeightMatrix(Vertices, adjMatrix);
    }

    public IEnumerable<int> GetAdjacentVertices(int fromVertice, int indexLimit = int.MaxValue)
    {
        if (!IndexIsInRange(fromVertice))
            throw new IndexOutOfRangeException("Vertice index is out of matrix");

        var targetRow = WeightMatrix[fromVertice];
        for (int i = 0; i < targetRow.Length && i < indexLimit; i++)
        {
            if (targetRow[i] >= 0)
                yield return i;
        }
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

    public void SetStartEndPoint(int startPointIndex, int endPointIndex)
    {
        FinishVerticeIndex = endPointIndex;
        IVertice finish = Vertices[endPointIndex];
        IVertice start = Vertices[startPointIndex];
        Parallel.ForEach(Vertices, vertice => vertice.SetHeuristic(start, finish));
    }

    public BilateralGraph(BilateralGraph original)
    {
        WeightMatrix = original.WeightMatrix;
        Vertices = new IVertice[original.Vertices.Length];
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] = original.Vertices[i].Copy();
        }
    }

    private bool IndexIsInRange(int index) => index >= 0 && index < WeightMatrix.Length;
}