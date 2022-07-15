namespace WebApp.Domain;

public interface IEmailSender
{
    Task SendMessageAsync(MailMessage message, MessageRecipientInfo reсipient, CancellationToken cancelToken = default);
}