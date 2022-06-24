using System.Collections.Concurrent;

namespace WebApp_MVC.Models;

public sealed class Catalog<T> where T : ICatalogItem
{
    private readonly ConcurrentDictionary<int, T> _items = new();
    private static int _key;
    private readonly object _syncObject = new();

    public int Count => _items.Count;

    public bool TryAdd(T item)
    {
        if (_items.TryAdd(_key, item))
        {
            Interlocked.Increment(ref _key);
            return true;
        }
        return false;
    }
    public bool Remove(long id)
    {
        lock (_syncObject)
        {
            foreach (var item in _items)
            {
                if (item.Value.Id == id)
                {
                    _items.Values.Remove(item.Value);
                    return true;
                }
            }

            return false;
        }
    }
}