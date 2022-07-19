using Microsoft.Extensions.Logging;
using WebApp.Domain;

namespace WebApp.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
    {
        ArgumentNullException.ThrowIfNull(productRepository);
        ArgumentNullException.ThrowIfNull(logger);

        _productRepository = productRepository;
    }

    public async Task Add(ProductToCreate item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(item);
        await _productRepository.Add(item, cancellationToken);
        DomainEventsManager.Raise(new ProductAdded(new ProductResponse
        {
            Title = item.Title,
            Image = item.Image,
        }));
    }
    public async Task<ProductResponse?> Get(long id, CancellationToken cancellationToken = default) 
        => await _productRepository.Get(id, cancellationToken);
    public async Task<IReadOnlyCollection<ProductResponse>> GetAll(CancellationToken cancellationToken = default) 
        => await _productRepository.GetAll(cancellationToken);
    public async Task<bool> Remove(long id, CancellationToken cancellationToken = default) 
        => await _productRepository.Remove(id, cancellationToken);
    public async Task<bool> Update(long id, ProductToUpdate newItem, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(newItem);
        return await _productRepository.Update(id, newItem, cancellationToken);
    }
}