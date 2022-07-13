using WebApp.Domain;

namespace WebApp.DAL;

public sealed class ProductRepository : IProductRepository
{
    private readonly Catalog<Product> _catalog = new();

    public ProductRepository(Catalog<Product> catalog)
    {
        _catalog = catalog;
    }

    /// <summary>
    /// Adding current product into the repository.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddAsync(ProductToCreate item, CancellationToken cancelToken = default)
   {
        ArgumentNullException.ThrowIfNull(item);

        var product = new Product
        {
            Title = item.Title,
            Image = item.Image,
        };

        await Task.Run(() =>
        {
            while (!_catalog.TryAdd(product));
        }, cancelToken);
    }
    /// <summary>
    /// Get product by id.
    /// </summary>
    /// <returns>
    /// Return the product if product with current id exist.
    /// Otherwise null.
    /// </returns>
    public async Task<ProductResponse> GetAsync(long id, CancellationToken cancelToken = default)
    {
        Product product = null!;
        await Task.Run(() => _catalog.TryGet((int)id, out product), cancelToken);

        if(product is null)
        {
            return null!;
        }

        return new ProductResponse
        {
            Title = product.Title,
            Id = product.Id,
            Image = product.Image,
        };
    }
    /// <summary>
    /// Get all products.
    /// </summary>
    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(CancellationToken cancelToken = default)
    {
        var products = await Task.Run(() => _catalog.Items, cancelToken);
        var productsResponse = new List<ProductResponse>(products.Count);

        foreach (var item in products)
        {
            productsResponse.Add(new ProductResponse
            {
                Id = item.Id,
                Title = item.Title,
                Image = item.Image,
            });
        }

        return productsResponse;
    }
    /// <summary>
    /// Removes product from repository by id.
    /// </summary>
    /// <returns>
    /// Returns true if the product exist. Otherwise false.
    /// </returns>
    public async Task<bool> RemoveAsync(long id, CancellationToken cancelToken = default)
    {
        return await Task.Run(() => _catalog.TryRemove((int)id), cancelToken);
    }
    /// <summary>
    /// Updated the product with current id.
    /// </summary>
    /// <returns>
    /// Returns true if product with current id exist.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> UpdateAsync(long id, ProductToUpdate newItem, CancellationToken cancelToken = default)
    {
        ArgumentNullException.ThrowIfNull(newItem);

        var product = new Product
        {
            Title = newItem.Title,
            Image = newItem.Image,
        };

        return await Task.Run(() => _catalog.TryUpdate((int)id, product), cancelToken);
    }
}