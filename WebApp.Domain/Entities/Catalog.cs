using System.Collections.Concurrent;

namespace WebApp.Domain;

public sealed class Catalog<T> where T : ICatalogItem, new()
{
    private readonly ConcurrentDictionary<int, T> _items = new();
    private static int _key;
    private readonly object _syncObject = new();

    public int Count => _items.Count;

    public IReadOnlyCollection<T> Items => (IReadOnlyCollection<T>)_items.Values;

    /// <summary>
    /// Tries to get value by id.
    /// </summary>
    /// <returns>
    /// true if the id was found in the catalog;
    /// otherwise, false.
    /// </returns>
    public bool TryGet(int id, out T value) => _items.TryGetValue(id, out value!);

    /// <summary>
    /// Tries to add value to the catalog.
    /// </summary>
    /// <returns>true if the key/value pair was added to the System.Collections.Concurrent.ConcurrentDictionary`2
    /// successfully; false if the key already exists.</returns>
    public bool TryAdd(T item)
    {
        if(_items.TryAdd(_key, item))
        {
            _items[_key].Id = _key;
            Interlocked.Increment(ref _key);
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// Tries to remove value from catalog by id.
    /// </summary>
    /// <returns>true when a value is found in the catalog with current id; false when
    /// the catalog cannot find a value associated with the specified id.</returns>
    public bool TryRemove(int id)
    {
        var item = _items[id];
        return _items.Remove(id, out item);
    }

    /// <summary>
    /// Tries to update item with current id.
    /// </summary>
    /// <returns>
    /// Returns true if the catalog contains item with current id.
    /// </returns>
    public bool TryUpdate(int id, T newItem)
    {
        if (!_items.ContainsKey(id))
        {
            lock (_syncObject)
            {
                _items[id] = newItem;
                _items[id].Id = id;
            }
            return true;
        }

        return false;
    }
}