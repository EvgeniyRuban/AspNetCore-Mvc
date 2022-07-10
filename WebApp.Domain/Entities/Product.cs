namespace WebApp.Domain;

public sealed class Product : ICatalogItem
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string? Image { get; set; }
}