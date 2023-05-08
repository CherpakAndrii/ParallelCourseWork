namespace Model.DataStructures;

public class MaxHeap<T> : Heap<T> where T: IComparable, IEquatable<T>
{
    public MaxHeap(int capacity = 0) : base(capacity){}
    public MaxHeap(ICollection<T> data) : base(data){}
    
    public MaxHeap(T[] data) : base(data){}
    protected override bool HasLowerPriority(T a, T b)
    {
        return a.CompareTo(b) >= 0;
    }

    protected override bool ContinueSearchCondition(T currentVal, T targetVal)
    {
        return currentVal.CompareTo(targetVal) >= 0;
    }
}