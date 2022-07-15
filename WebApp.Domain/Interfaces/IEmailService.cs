namespace WebApp.Domain;

public interface IEmailService
{
    Task SendMessageAsync(MailMessage message, MessageRecipientInfo reсipient, CancellationToken cancelToken = default);
}