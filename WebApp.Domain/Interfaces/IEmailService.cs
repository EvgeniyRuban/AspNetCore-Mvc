namespace WebApp.Domain;

public interface IEmailService
{
    MessageSenderInfo Sender { get; set; }
    void SendMessage(MailMessage message, params MessageRecipientInfo[] reсipients);
}