namespace WebApp.Domain;

public sealed class MessageSenderInfo
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Host { get; set; } = null!;
    public int Port { get; set; }
}