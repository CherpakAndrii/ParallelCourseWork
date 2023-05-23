namespace Model.Entities;

public partial interface IGraph
{
    public float[][] WeightMatrix { get; set; }
    public IVertice[] Vertices { get; }
    public int StartVerticeIndex { get; set; }
    public int FinishVerticeIndex { get; set; }

    public void Reset()
    {
        Parallel.ForEach(Vertices, v => v.Reset());
    }
    
    public enum FileType
    {
        Binary,
        Text
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

    public void SetStartEndPoint(int startPoint, int endPointIndex);

    public bool IndexIsInRange(int index) => index >= 0 && index < WeightMatrix.Length;
}