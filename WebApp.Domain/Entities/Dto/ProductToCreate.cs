namespace WebApp.Domain;

public sealed class ProductToCreate
{
    public string Title { get; set; } = null!;
    public string? Image { get; set; }
}
