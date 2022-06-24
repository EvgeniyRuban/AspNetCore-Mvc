namespace WebApp_MVC.Models;

public sealed class Catalog
{
    private readonly object _syncObject = new();
    private List<Product> _items = new();

    public IReadOnlyList<Product> Items => _items;

    public void Add(Product product)
    {
        lock (_syncObject)
        {
            _items.Add(product);
        }
    }
}