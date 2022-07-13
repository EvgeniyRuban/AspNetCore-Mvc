namespace WebApp.Domain;

public interface IEmailService
{
    void SendMessage(MailMessage message, params MessageRecipientInfo[] reсipients);
}