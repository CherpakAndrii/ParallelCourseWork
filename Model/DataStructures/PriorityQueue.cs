using System.Collections;
using System.Runtime.CompilerServices;

namespace Model.DataStructures;

public partial class PriorityQueue<T> : IPriorityQueue<T> where T : IComparable, IEquatable<T>
{
    private MinHeap<IPriorityQueue<T>.QueueNode> _heap;
    public PriorityQueue(int capacity) => _heap = new MinHeap<IPriorityQueue<T>.QueueNode>(capacity);
    public PriorityQueue() => _heap = new MinHeap<IPriorityQueue<T>.QueueNode>();

    public PriorityQueue(ICollection<(T, float)> data)
    {
        List<IPriorityQueue<T>.QueueNode> src = new List<IPriorityQueue<T>.QueueNode>();
        foreach (var pair in data) src.Add(pair);
        _heap = new MinHeap<IPriorityQueue<T>.QueueNode>(src);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(_heap.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        _heap.Add((item, short.MaxValue));
    }

    public void Clear() => _heap.Clear();

    public bool Contains(T item)
    {
        return _heap.Contains((item, short.MaxValue));
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        return _heap.Remove((item, short.MaxValue));
    }
    
    public void Enqueue(T value, float priority) => Enqueue((value, priority));

    public void Enqueue((T, float) item)
    {
        _heap.Add(item);
    }
    
    public bool EnqueueWithReplacement(T value, float priority) => EnqueueWithReplacement((value, priority));

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool EnqueueWithReplacement((T, float) item)
    {
        return _heap.AddWithReplacement(item);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public T Dequeue()
    {
        return _heap.Pop().Data;
    }

    public int Count => _heap.Count;
    public bool IsReadOnly => _heap.IsReadOnly;
}
    