using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using WebApp.Domain;

namespace WebApp.Services;

public sealed class ProductService : IProductService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IProductRepository _productRepository = null!;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        ArgumentNullException.ThrowIfNull(productRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary></summary>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddAsync(ProductToCreate item, CancellationToken cancelToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        _logger.LogDebug("Requested addition of the {@item} to repository", item);

        try
        {
            await _productRepository.AddAsync(item, cancelToken);
        }
        catch(TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return;
        }

        _logger.LogDebug("{@item} added.", item);
    }

    /// <summary></summary>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<ProductResponse?> GetAsync(long id, CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested product with id: {id}", id);
        ProductResponse product = null!;
        try
        {
            product = await _productRepository.GetAsync(id, cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return null;
        }

        if(product is null)
        {
            _logger.LogDebug("Requested product was not found {@product}", product);
        }
        else
        {
            _logger.LogDebug("Requested product was found {@product}", product);
        }
        
        return product;
    }

    /// <summary></summary>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested all products from repository.");
        IReadOnlyCollection<ProductResponse> result = null!;
        try
        {
            result = await _productRepository.GetAllAsync(cancelToken);
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
        
        _logger.LogDebug("All products returned.");
        return result;
    }

    /// <summary></summary>
    /// <exception cref="TaskCanceledException"></exception>
    public async Task<bool> RemoveAsync(long id, CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested to remove a product repository by {id}.", id);

        var result = false;
        try
        {
            await _productRepository.RemoveAsync(id, cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch (Exception ex)
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

    /// <summary></summary>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> UpdateAsync(long id, ProductToUpdate newItem, CancellationToken cancelToken = default)
    {
        _logger.LogDebug("Requested product update with id: {id}, to {@newItem}.", id, newItem);
        ArgumentNullException.ThrowIfNull(newItem);
        
        bool result = false;

        try
        {
            await _productRepository.UpdateAsync(id, newItem, cancelToken);
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("Task canceled.");
            throw new TaskCanceledException();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return false;
        }

        return result;
    }
}