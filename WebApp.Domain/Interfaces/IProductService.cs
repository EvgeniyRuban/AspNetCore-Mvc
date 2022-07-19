namespace WebApp.Domain;

public interface IProductService
{
    /// <summary></summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task Add(ProductToCreate item, CancellationToken cancellationToken = default);
    /// <summary></summary>
    /// <exception cref="OperationCanceledException"></exception>
    Task<ProductResponse?> Get(long id, CancellationToken cancellationToken = default);
    /// <summary></summary>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IReadOnlyCollection<ProductResponse>> GetAll(CancellationToken cancellationToken = default);
    /// <summary></summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> Update(long id, ProductToUpdate newItem, CancellationToken cancellationToken = default);
    /// <summary></summary>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> Remove(long id, CancellationToken cancellationToken = default);
}