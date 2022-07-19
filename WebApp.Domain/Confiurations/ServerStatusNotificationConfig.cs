namespace WebApp.Domain;

public sealed class ServerStatusNotificationConfig
{
    public string RecipientAddress { get; set; } = null!;
    public string RecipientName { get; set; } = null!;
    public string MessageSubject { get; set; } = null!;
    public string MessageBody { get; set; } = null!;
}
