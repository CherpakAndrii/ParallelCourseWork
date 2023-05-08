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
        bool[][] _adjMatrix = GraphGenerator.GenerateAdjacenceMatrix(size);
        WeightMatrix = BuildWeightMatrix(Vertices, _adjMatrix);
    }

    public void Reset()
    {
        foreach (var v in Vertices)
        {
            v.Reset();
        }
    }

    public Graph(string sourceFilePath)
    {
        (bool[][] _adjMatrix, (int, int)[] coordinates) = ReadFromFile(sourceFilePath);
        Vertices = new Vertice[coordinates.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            Vertices[i] = new Vertice(this, i, coordinates[i]);
        }
        WeightMatrix = BuildWeightMatrix(Vertices, _adjMatrix);
    }

    public IEnumerable<int> GetAdjacentVertices(int fromVertice)
    {
        if (!IndexIsInRange(fromVertice))
            throw new IndexOutOfRangeException("Vertice index is out of matrix");

        var targetRow = WeightMatrix[fromVertice];
        for (int i = 0; i < targetRow.Length; i++)
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
        foreach (var vertice in Vertices)
        {
            vertice.SetHeuristic(Vertices[endPointIndex]);
        }
    }

    private bool IndexIsInRange(int index) => index >= 0 && index < WeightMatrix.Length;
}