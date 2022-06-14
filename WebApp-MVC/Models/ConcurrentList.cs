using System.Collections;

namespace WebApp_MVC.Models;

public sealed class ConcurrentList<T> : IList<T>
{
    private readonly List<T> _list = new();
    private readonly object _syncObject = new();

    public T this[int index]
    {
        get => _list[index];
        set => _list[index] = value; // ???
    }

    public int Count => _list.Count;
    public bool IsReadOnly => default;
        
    public void Add(T item)
    {
        lock (_syncObject)
        {
            _list.Add(item);
        }
    }
    public void Clear()
    {
        lock (_syncObject)
        {
            _list.Clear();
        }
    }
    public bool Contains(T item)
    {
        lock (_syncObject)
        {
            return _list.Contains(item);
        }
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_syncObject)
        {
            _list.CopyTo(array, arrayIndex);
        }
    }
    public IEnumerator<T> GetEnumerator()
    {
        lock (_syncObject)
        {
            return _list.GetEnumerator();
        }
    }
    public int IndexOf(T item)
    {
        lock (_syncObject)
        {
            return _list.IndexOf(item);
        }
    }
    public void Insert(int index, T item)
    {
        lock (_syncObject)
        {
            _list.Insert(index, item);
        }
    }
    public bool Remove(T item)
    {
        lock (_syncObject)
        {
            return _list.Remove(item);
        }
    }
    public void RemoveAt(int index)
    {
        lock (_syncObject)
        {
            _list.RemoveAt(index);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        lock (_syncObject)
        {
            return GetEnumerator(); // ???
        }
    }
}