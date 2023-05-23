namespace Model.Entities;

public interface IVertice
{
    public Coordinates VerticeCoordinates { get; }
    
    public int OwnIndex { get; }

    public void Reset();
    public IVertice Copy();
    public void SetHeuristic(IVertice start, IVertice finish);
}