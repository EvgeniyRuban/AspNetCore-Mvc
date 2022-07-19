using Microsoft.Extensions.Logging;
using WebApp.Domain;

namespace WebApp.DAL;

public sealed class ProductRepository : IProductRepository
{
    private readonly ILogger<ProductRepository> _logger;
    private readonly Catalog<Product> _catalog = new();

    public ProductRepository(Catalog<Product> catalog, ILogger<ProductRepository> logger)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(logger);

        _catalog = catalog;
        _logger = logger;
    }

    public async Task Add(ProductToCreate item, CancellationToken cancellationToken = default)
   {
        ArgumentNullException.ThrowIfNull(item);

        _logger.LogDebug("Requested to be added to catalog.", item);

        var product = new Product
        {
            Title = item.Title,
            Image = item.Image,
        };

        cancellationToken.ThrowIfCancellationRequested();
        await Task.Run(() => _catalog.TryAdd(product));

        _logger.LogDebug("{@item} added to catalog.", item);
    }
    public async Task<ProductResponse?> Get(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requested product with id: {id}", id);
        Product product = null!;

        cancellationToken.ThrowIfCancellationRequested();
        await Task.Run(() => _catalog.TryGet((int)id, out product));

        if(product is null)
        {
            _logger.LogInformation("Product with current id: {id} not exist!", id);
            return null;
        }

        _logger.LogDebug("Requested product was found {@product}", product);

        return new ProductResponse
        {
            Title = product.Title,
            Image = product.Image,
        };
    }
    public async Task<IReadOnlyCollection<ProductResponse>> GetAll(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requested all catalog of products.");
        IReadOnlyCollection<Product> products = null!;

        cancellationToken.ThrowIfCancellationRequested();
        products = await Task.Run(() => _catalog.Items);
            
        var productsResponse = new List<ProductResponse>(products.Count);

        foreach (var item in products)
        {
            productsResponse.Add(new ProductResponse
            {
                Title = item.Title,
                Image = item.Image,
            });
        }

        _logger.LogDebug("All products returned.");

        return productsResponse;
    }
    public async Task<bool> Remove(long id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requested to remove a product from the catalog by {id}.", id);
        bool result = false;

        cancellationToken.ThrowIfCancellationRequested();
        result = await Task.Run(() => _catalog.TryRemove((int)id));

        if (result)
        {
            _logger.LogDebug("Product deleting succes.");
        }
        else
        {
            _logger.LogDebug("Product deleting failed.");
        }

        return result;
    }
    public async Task<bool> Update(
        long id, 
        ProductToUpdate newItem, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Requested product update with id: {id}, to {@newItem}.", id, newItem);

        ArgumentNullException.ThrowIfNull(newItem);

        bool result = false;
        var product = new Product
        {
            Title = newItem.Title,
            Image = newItem.Image,
        };

        cancellationToken.ThrowIfCancellationRequested();
        result = await Task.Run(() => _catalog.TryUpdate((int)id, product));
        

        if (result)
        {
            _logger.LogDebug("Product updating succes.");
        }
        else 
        { 
            _logger.LogDebug("Product updating failed.");
        }

        return result;
    }
}