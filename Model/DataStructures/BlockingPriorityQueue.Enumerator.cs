using System.Collections;

namespace Model.DataStructures;

public partial class BlockingPriorityQueue<T>
{
    private class Enumerator : IEnumerator<T>
    {
        private IEnumerator<IPriorityQueue<T>.QueueNode> _heapEnumerator;
        public Enumerator(IEnumerator<IPriorityQueue<T>.QueueNode> heapEnumerator)
        {
            _heapEnumerator = heapEnumerator;
        }

        public bool MoveNext()
        {
            return _heapEnumerator.MoveNext();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public T Current => _heapEnumerator.Current.Data;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            
        }
    }
}