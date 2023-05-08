using System.Collections;

namespace Model.DataStructures;

public partial class BlockingPriorityQueue<T> : IPriorityQueue<T> where T : IComparable, IEquatable<T>
{
    private MinHeap<IPriorityQueue<T>.QueueNode> _heap;
    readonly object queueLock = new object();
    
    public BlockingPriorityQueue(int capacity) => _heap = new MinHeap<IPriorityQueue<T>.QueueNode>(capacity);
    public BlockingPriorityQueue() => _heap = new MinHeap<IPriorityQueue<T>.QueueNode>();

    public BlockingPriorityQueue(ICollection<(T, float)> data)
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
        bool contains = false;
        while (!Monitor.TryEnter(queueLock))
        {
            Monitor.Wait(queueLock, new TimeSpan(0, 0, 10));
        }

        try
        {
            contains = _heap.Contains((item, short.MaxValue));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            contains = default;
        }
        finally
        {
            Monitor.Exit(queueLock);
            Monitor.PulseAll(queueLock);
        }

        return contains;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        bool removed = false;
        while (!Monitor.TryEnter(queueLock))
        {
            Monitor.Wait(queueLock, new TimeSpan(0, 0, 10));
        }

        try
        {
            removed = _heap.Remove((item, short.MaxValue));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            removed = default;
        }
        finally
        {
            Monitor.Exit(queueLock);
            Monitor.PulseAll(queueLock);
        }

        return removed;
    }
    
    public void Enqueue(T value, float priority) => Enqueue((value, priority));

    public void Enqueue((T, float) item)
    {
        while (!Monitor.TryEnter(queueLock))
        {
            Monitor.Wait(queueLock, new TimeSpan(0, 0, 10));
        }

        try
        {
            _heap.Add(item);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            Monitor.PulseAll(queueLock);
            Monitor.Exit(queueLock);
        }
    }
    
    public bool EnqueueWithReplacement(T value, float priority) => EnqueueWithReplacement((value, priority));

    public bool EnqueueWithReplacement((T, float) item)
    {
        bool replaced = false;
        while (!Monitor.TryEnter(queueLock))
        {
            Monitor.Wait(queueLock, new TimeSpan(0, 0, 10));
        }

        try
        {
            replaced = _heap.AddWithReplacement(item);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            replaced = default;
        }
        finally
        {
            Monitor.Exit(queueLock);
            Monitor.PulseAll(queueLock);
        }

        return replaced;
    }

    public T Dequeue()
    {
        T data;
        while (!Monitor.TryEnter(queueLock))
        {
            Monitor.Wait(queueLock, new TimeSpan(0, 0, 10));
        }
        try
        {
            data = _heap.Pop().Data;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            data = default;
        }
        finally
        {
            Monitor.PulseAll(queueLock);
            Monitor.Exit(queueLock);
        }
        
        return data;
    }

    public int Count => _heap.Count;
    public bool IsReadOnly => _heap.IsReadOnly;
}