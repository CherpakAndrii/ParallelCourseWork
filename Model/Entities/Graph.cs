namespace Model.Entities;

public partial class Graph
{
    public float[][] WeightMatrix { get; set; }
    public Vertice[] Vertices { get; }
    public int FinishVerticeIndex { get; private set; }

    public Graph(int size)
    {
        if (size < 3)
            throw new ArgumentException("The graph must contain at least 3 vertices!");

        Vertices = GraphGenerator.GenerateVertices(size, this);
        Console.WriteLine("Vertices generated!");
        bool[][] _adjMatrix = GraphGenerator.GenerateAdjacenceMatrix(size, 0.3f);
        Console.WriteLine("Adj matrix generated!");
        WeightMatrix = BuildWeightMatrix(Vertices, _adjMatrix);
        Console.WriteLine("Weight matrix built!");
    }

    public void Reset()
    {
        Parallel.ForEach(Vertices, v => v.Reset());
    }
    
    public enum FileType
    {
        Binary,
        Text
    }

    public Graph(string sourceFilePath, FileType fileType)
    {
        (bool[][] adjMatrix, (int, int)[] coordinates) = fileType == FileType.Text
            ? ReadFromTxtFile(sourceFilePath)
            : ReadFromBinFile(sourceFilePath);
        Vertices = new Vertice[coordinates.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            Vertices[i] = new Vertice(this, i, coordinates[i]);
        }
        WeightMatrix = BuildWeightMatrix(Vertices, adjMatrix);
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

    public Vertice this[int ind]
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

    public void SetEndPoint(int endPointIndex)
    {
        FinishVerticeIndex = endPointIndex;
        Vertice finish = Vertices[endPointIndex];
        Parallel.ForEach(Vertices, vertice => vertice.SetHeuristic(finish));
    }

    public Graph(Graph original)
    {
        WeightMatrix = original.WeightMatrix;
        Vertices = new Vertice[original.Vertices.Length];
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] = original.Vertices[i].Copy();
        }
    }

    private bool IndexIsInRange(int index) => index >= 0 && index < WeightMatrix.Length;
}