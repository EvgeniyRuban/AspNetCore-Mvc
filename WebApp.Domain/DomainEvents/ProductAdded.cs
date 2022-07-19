namespace WebApp.Domain;

public sealed class ProductAdded : IDomainEvent
{
    public ProductAdded(ProductResponse product) => Product = product;

    public ProductResponse Product { get; set; }
}