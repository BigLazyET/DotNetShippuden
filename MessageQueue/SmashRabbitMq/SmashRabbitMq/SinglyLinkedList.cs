using System.Collections;

namespace SmashRabbitMq;

public class SinglyLinkedList<T> : IEnumerable<T> where T: class
{
    private SinglyLinkedNode<T> _head;
    private SinglyLinkedNode<T> _tail;

    public SinglyLinkedNode<T> Head => _head;

    public SinglyLinkedList()
    {
    }

    public SinglyLinkedList(SinglyLinkedNode<T> head)
    {
        _head = head;
        _tail = head;
    }

    public void AddLast(T value)
    {
        var newNode = new SinglyLinkedNode<T>(value);
        if (_head == null)
        {
            _head = newNode;
            _tail = newNode;
        }
        else
        {
            _tail.Next = newNode;
            _tail = newNode;
        }
    }

    public bool Delete(T value)
    {
        if (_head == null) return false;

        if (_head.Value == value)
        {
            _head = _head.Next;
            return true;
        }

        var current = _head;
        if (current.Next != null && current.Next.Value != value)
        {
            current = current.Next;
        }

        if (current == null) return false;
        current = current.Next.Next;
        return true;
    }

    public T this[int Index]
    {
        get
        {
            if (Index < 0) throw new ArgumentOutOfRangeException(nameof(Index));
            
            var count = 0;

            var current = Head;
            while (current != null)
            {
                if (count == Index)
                    return current.Value;
                current = current.Next;
                count++;
            }

            throw new ArgumentOutOfRangeException(nameof(Index));
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        var current = Head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class SinglyLinkedNode<T>
{
    public T Value { get; }
    public SinglyLinkedNode<T> Next { get; set; }

    public SinglyLinkedNode(T value)
    {
        Value = value;
    }
}

public class Node : EqualityComparer<Node>
{
    public NodeBindType Type { get; set; }
    
    public string BindFrom { get; set; }
    
    public string BindTo { get; set; }
    
    public string RoutingKey { get; set; }
    
    public override bool Equals(Node? x, Node? y)
    {
        if (x == null || y == null) return false;
        return x.Type == y.Type && x.BindFrom == y.BindFrom && x.BindTo == y.BindTo && x.RoutingKey == y.RoutingKey;
    }

    public override int GetHashCode(Node obj)
    {
        return obj.GetHashCode();
    }
}

public enum NodeDeclareType
{
    Exchange,
    Queue
}

public enum NodeBindType
{
    Exchange,
    Queue
}