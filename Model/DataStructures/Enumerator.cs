using System.Collections;

namespace Model.DataStructures;

public partial class Heap<T>
{
    public class Enumerator : IEnumerator<T>
    {
        private T _current;
        private Heap<T> _heap;
        public Enumerator(Heap<T> heap)
        {
            _current = default;
            _heap = heap;
        }

        public bool MoveNext()
        {
            try
            {
                _current = _heap.Pop();
                return true;
            }
            catch (IndexOutOfRangeException e)
            {
                return false;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public T Current => _current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            
        }
    }
}