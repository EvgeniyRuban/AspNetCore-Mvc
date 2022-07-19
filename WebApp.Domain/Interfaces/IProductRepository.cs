namespace WebApp.Domain;

public interface IProductRepository
{
    /// <summary>
    /// Adding current product into the catalog.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="OperationCanceledException"></exception>
    Task Add(ProductToCreate item, CancellationToken cancellationToken = default);
    /// <summary>
    /// Get product by id.
    /// </summary>
    /// <returns>
    /// Return the product if product with current id exist.
    /// </returns>
    ///<exception cref="OperationCanceledException"></exception> 
    Task<ProductResponse?> Get(long id, CancellationToken cancellationToken = default);
    /// <summary>
    /// Get all products.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<IReadOnlyCollection<ProductResponse>> GetAll(CancellationToken cancellationToken = default);
    /// <summary>
    /// Updated the product with current id.
    /// </summary>
    /// <returns>
    /// Returns true if product with current id exist. Otherwise false.
    /// </returns>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task<bool> Update(long id, ProductToUpdate newItem, CancellationToken cancellationToken = default);
    /// <summary>
    /// Removes product from catalog by id.
    /// </summary>
    /// <returns>
    /// Returns true if the product exist. Otherwise false.
    /// </returns>
    /// <exception cref="OperationCanceledException"></exception>
    Task<bool> Remove(long id, CancellationToken cancellationToken = default);
}