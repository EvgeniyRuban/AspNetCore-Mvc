namespace WebApp.Domain;

public interface IEmailSender
{
    Task SendMessage(
        MailMessage message, 
        MessageRecipientInfo reсipient, 
        CancellationToken cancellationToken = default);
}