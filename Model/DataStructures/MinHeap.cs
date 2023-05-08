namespace Model.DataStructures;

public class MinHeap<T> : Heap<T> where T: IComparable, IEquatable<T>
{
    public MinHeap(int capacity = 0) : base(capacity){}
    public MinHeap(ICollection<T> data) : base(data){}
    public MinHeap(T[] data) : base(data){}

    protected override bool HasLowerPriority(T a, T b)
    {
        return a.CompareTo(b) <= 0;
    }

    protected override bool ContinueSearchCondition(T currentVal, T targetVal)
    {
        return currentVal.CompareTo(targetVal) <= 0;
    }
}