namespace Model.DataStructures;

public partial interface IPriorityQueue<T>
{
    public class QueueNode : IComparable, IEquatable<QueueNode>
    {
        public float Priority { get; protected set; }
        public T Data { get; protected set; }

        public QueueNode(T data, float priority)
        {
            Data = data;
            Priority = priority;
        }
        
        public int CompareTo(object? value)
        {
            if (value == null)
                return 1;
            if (!(value is QueueNode node))
                throw new ArgumentException();
            return Priority > node.Priority? 1 : Priority < node.Priority ? -1 : 0;
        }

        public static implicit operator QueueNode((T, float) source) => new QueueNode(source.Item1, source.Item2);
        public bool Equals(QueueNode? other)
        {
            if (other == null)
                return false;
            return ((Object)Data).Equals(other.Data);
        }

        public override string ToString()
        {
            return Data is null? "Null" : Data.ToString();
        }
    }
}