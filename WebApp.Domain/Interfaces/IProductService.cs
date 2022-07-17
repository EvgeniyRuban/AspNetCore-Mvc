namespace WebApp.Domain;

public interface IProductService
{
    Task Add(ProductToCreate item, CancellationToken cancellationToken = default);
    Task<ProductResponse> Get(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ProductResponse>> GetAll(CancellationToken cancellationToken = default);
    Task<bool> Update(long id, ProductToUpdate newItem, CancellationToken cancellationToken = default);
    Task<bool> Remove(long id, CancellationToken cancellationToken = default);
}