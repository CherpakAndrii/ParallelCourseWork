using System.Collections;

namespace Model.DataStructures;

public abstract partial class Heap<T> : ICollection<T> where T : IComparable, IEquatable<T>
{
    public int Count { get; protected set; }
    public bool IsReadOnly { get; protected set; }
    protected T[] _data;
    protected int _capacity;
    protected int _lastFreeIndex;

    public Heap(int capacity = 0)
    {
        if (capacity < 0) throw new ArgumentException();
        Count = 0;
        _capacity = capacity;
        _data = new T[_capacity];
        _lastFreeIndex = _capacity - 1;
        IsReadOnly = false;
    }
    
    public Heap(ICollection<T> data)
    {
        Count = data.Count;
        _capacity = data.Count;
        _data = new T[_capacity];
        IsReadOnly = false;
        data.CopyTo(_data, 0);
        _lastFreeIndex = -1;
        for (int ctr = (Count >> 1)-1; ctr >= 0; ctr--) Heapify(ctr);
    }
    
    public Heap(T[] data)
    {
        Count = data.Length;
        _capacity = data.Length;
        _data = new T[_capacity];
        IsReadOnly = false;
        data.CopyTo(_data, 0);
        _lastFreeIndex = -1;
        for (int ctr = (Count >> 1)-1; ctr >= 0; ctr--) Heapify(ctr);
    }

    public void Add(T item)
    {
        if (_lastFreeIndex < 0) Resize();
        _data[_lastFreeIndex] = item;
        _lastFreeIndex--;
        Count++;
        Heapify(0);
    }
    
    public bool AddWithReplacement(T item)
    {
        bool oldRemoved = Remove(item);
        if (_lastFreeIndex < 0) Resize();
        _data[_lastFreeIndex] = item;
        _lastFreeIndex--;
        Count++;
        Heapify(0);
        return oldRemoved;
    }

    public void Clear()
    {
        Count = 0;
        _capacity = 10;
        _data = new T[_capacity];
        _lastFreeIndex = _capacity - 1;
    }

    public bool Contains(T item)
    {
        return FindInternalIndexByItem(item) != -1;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _data[(_lastFreeIndex+1)..(_lastFreeIndex+Count)].CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        int internalIndex = FindInternalIndexByItem(item);
        if (internalIndex == -1) return false;
        RemoveAtInternalIndex(internalIndex);
        return true;
    }

    public T Pop()
    {
        if (Count == 0) throw new IndexOutOfRangeException();
        T max = this[0];
        RemoveAtInternalIndex(0);
        return max;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    protected abstract bool HasLowerPriority(T a, T b);

    protected void Heapify(int ind)
    {
        if (Count > 2 * ind + 1)
        {
            int a = 2*ind+1, b = 2*ind+2;
            int maxChildIndex = Count == b || HasLowerPriority(this[a], this[b])? a : b;
            if (HasLowerPriority(this[maxChildIndex], this[ind]))
            {
                (this[maxChildIndex], this[ind]) = (this[ind], this[maxChildIndex]);
                Heapify(maxChildIndex);
            }
        }
    }
    
    protected void Resize(bool shrink = false)
    {
        if (shrink)
        {
            _data = _data[(_lastFreeIndex+1)..(_lastFreeIndex+Count)];
            _capacity = Count;
            _lastFreeIndex = -1;
            return;
        }

        if (_capacity == 0)
        {
            _capacity = 10;
            _data = new T[_capacity];
            _lastFreeIndex += _capacity;
        }
        else
        {
            T[] newData = new T[_capacity * 2];
            _lastFreeIndex += _capacity;
            _data.CopyTo(newData, _lastFreeIndex+1);
            _capacity *= 2;
            _data = newData;
        }
    }

    protected T this[int index]
    {
        get
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            return _data[_lastFreeIndex + 1 + index];
        }
        set
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
            _data[_lastFreeIndex + 1 + index] = value;
        }
    }

    protected void RemoveAtInternalIndex(int index)
    {
        if (index < 0 || index >= Count) throw new IndexOutOfRangeException();
        (this[index], this[Count - 1]) = (this[Count - 1], this[index]);
        Count--;
        Heapify(index);
    }

    protected abstract bool ContinueSearchCondition(T currentVal, T targetVal);

    protected int FindInternalIndexByItem(T item)
    {
        for (int i = 0; i < Math.Log2(Count); i++)
        {
            bool redFlag = true;
            int currentLineSize = (int)Math.Pow(2, i);
            for (int ctr = 0; ctr < currentLineSize && currentLineSize + ctr <= Count; ctr++)
            {
                T val = this[currentLineSize - 1 + ctr];
                if (val.Equals(item)) return currentLineSize - 1 + ctr;
                if (ContinueSearchCondition(val, item)) redFlag = false;
            }

            if (redFlag) break;
        }
        return -1;
    }

    public T[] ToSortedArray()
    {
        Resize(true);
        T[] backup = new T[Count];
        CopyTo(backup, 0);
        while (Count > 0) Pop();

        T[] result = _data;
        _data = backup;
        Count = _capacity = backup.Length;

        return result;
    }
}