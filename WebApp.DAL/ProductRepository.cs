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

    /// <summary>
    /// Adding current product into the repository.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddAsync(ProductToCreate item, CancellationToken cancelToken = default)
   {
        ArgumentNullException.ThrowIfNull(item);

        _logger.LogDebug("Requested to be added to catalog.", item);

        var product = new Product
        {
            Title = item.Title,
            Image = item.Image,
        };

        try
        {
            await Task.Run(() => _catalog.TryAdd(product), cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return;
        }

        _logger.LogDebug("{@item} added to catalog.", item);
    }

    /// <summary>
    /// Get product by id.
    /// </summary>
    /// <returns>
    /// Return the product if product with current id exist.
    /// </returns>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<ProductResponse?> GetAsync(long id, CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested product with id: {id}", id);
        Product product = null!;

        try
        {
            await Task.Run(() => _catalog.TryGet((int)id, out product), cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }
        

        if(product is null)
        {
            _logger.LogInformation("Product with current id: {id} not exist!", id);
            return null;
        }

        _logger.LogDebug("Requested product was found {@product}", product);

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
    /// <returns></returns>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested all catalog of products.");
        IReadOnlyCollection<Product> products = null!;

        try
        {
            products = await Task.Run(() => _catalog.Items, cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null!;
        }
            
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

        _logger.LogDebug("All products returned.");

        return productsResponse;
    }

    /// <summary>
    /// Removes product from repository by id.
    /// </summary>
    /// <returns>
    /// Returns true if the product exist. Otherwise false.
    /// </returns>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<bool> RemoveAsync(long id, CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested to remove a product from the catalog by {id}.", id);
        bool result = false;

        try
        {
            result = await Task.Run(() => _catalog.TryRemove((int)id), cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }

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

    /// <summary>
    /// Updated the product with current id.
    /// </summary>
    /// <returns>
    /// Returns true if product with current id exist.
    /// </returns>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> UpdateAsync(long id, ProductToUpdate newItem, CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested product update with id: {id}, to {@newItem}.", id, newItem);

        ArgumentNullException.ThrowIfNull(newItem);

        bool result = false;
        var product = new Product
        {
            Title = newItem.Title,
            Image = newItem.Image,
        };

        try
        {
            result = await Task.Run(() => _catalog.TryUpdate((int)id, product), cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }

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