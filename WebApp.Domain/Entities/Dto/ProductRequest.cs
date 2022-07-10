namespace WebApp.Domain;

public sealed class ProductRequest
{
    public string Title { get; set; } = null!;
    public string? Image { get; set; }
}