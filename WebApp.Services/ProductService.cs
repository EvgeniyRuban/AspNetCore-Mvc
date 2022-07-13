using WebApp.Domain;

namespace WebApp.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository = null!;

    public ProductService(IProductRepository productRepository)
    {
        ArgumentNullException.ThrowIfNull(productRepository);
        _productRepository = productRepository;
    }

    public async Task AddAsync(ProductToCreate item, CancellationToken cancelToken = default)
    {
        await _productRepository.AddAsync(item, cancelToken);
    }
    public async Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(CancellationToken cancelToken = default)
    {
        return await _productRepository.GetAllAsync(cancelToken);
    }
    public async Task<ProductResponse> GetAsync(long id, CancellationToken cancelToken = default)
    {
        return await _productRepository.GetAsync(id, cancelToken);
    }
    public async Task<bool> RemoveAsync(long id, CancellationToken cancelToken = default)
    {
        return await _productRepository.RemoveAsync(id, cancelToken);
    }
    public async Task<bool> UpdateAsync(long id, ProductToUpdate newItem, CancellationToken cancelToken = default)
    {
        return await _productRepository.UpdateAsync(id, newItem, cancelToken);
    }
}