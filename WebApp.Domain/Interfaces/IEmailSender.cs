namespace WebApp.Domain;

public interface IEmailSender
{
    /// <summary>
    /// Sending a message to the recipient.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    Task SendMessage(
        MailMessage message, 
        MessageRecipientInfo reсipient, 
        CancellationToken cancellationToken = default);
}