using System;
using System.Collections.Generic;

public class PrataStack<T>
{
    private readonly LinkedList<T> list = new LinkedList<T>();

    public void Push(T value)
    {
        list.AddLast(value);
    }

    public int Count { get { return list.Count; } }

    public T Pop()
    {
        LinkedListNode<T> node = list.Last;
        if (node == null) throw new InvalidOperationException();
        list.RemoveLast();
        return node.Value;
    }

    public bool Remove(T item) => list.Remove(item);

    public bool Contains(T other) => list.Contains(other);

    public void Clear() => list.Clear();
}
