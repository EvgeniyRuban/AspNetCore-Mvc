namespace WebApp.Domain;

public sealed class MailMessage
{
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}