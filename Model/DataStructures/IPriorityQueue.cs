namespace Model.DataStructures;

public partial interface IPriorityQueue<T> : ICollection<T> where T : IComparable, IEquatable<T>
{
    public void Enqueue(T value, float priority);
    public void Enqueue((T, float) item);
    public bool EnqueueWithReplacement(T value, float priority);
    
    public bool EnqueueWithReplacement((T, float) item);

    public T Dequeue();
}