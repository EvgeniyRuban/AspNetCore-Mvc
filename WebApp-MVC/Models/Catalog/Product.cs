namespace WebApp_MVC.Models;

public sealed class Product : ICatalogItem
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string? Image { get; set; }
}