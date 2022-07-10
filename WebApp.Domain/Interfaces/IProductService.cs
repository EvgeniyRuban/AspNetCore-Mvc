namespace WebApp.Domain;

public interface IProductService
{
    Task AddAsync(ProductToCreate item, CancellationToken cancelToken = default);
    Task<ProductResponse> GetAsync(long id, CancellationToken cancelToken = default);
    Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(CancellationToken cancelToken = default);
    Task<bool> UpdateAsync(long id, ProductToUpdate newItem, CancellationToken cancelToken = default);
    Task<bool> RemoveAsync(long id, CancellationToken cancelToken = default);
}