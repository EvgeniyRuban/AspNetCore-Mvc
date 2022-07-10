namespace WebApp.Domain;

public sealed class ProductResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Image { get; set; }
}
