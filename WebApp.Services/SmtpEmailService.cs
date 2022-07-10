using MailKit.Net.Smtp;
using MimeKit;
using WebApp.Domain;

namespace WebApp.Services;

public sealed class SmtpEmailService : IEmailService
{
    private MessageSenderInfo _sender = null!;

    public MessageSenderInfo Sender 
    {
        get => _sender;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (string.IsNullOrEmpty(value.Address)
            || string.IsNullOrEmpty(value.Login)
            || string.IsNullOrEmpty(value.Name)
            || string.IsNullOrEmpty(value.Password)
            || string.IsNullOrEmpty(value.Host))
            {
                throw new ArgumentNullException("Not all sender properties has value!");
            }

            _sender = value;
        }
    }

    /// <summary>
    /// Sending a message to all of recipients.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public void SendMessage(MailMessage message, params MessageRecipientInfo[] reсipients)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (reсipients is null)
        {
            throw new ArgumentNullException(nameof(reсipients));
        }

        if(reсipients.Length == 0)
        {
            throw new ArgumentException($"The number of recipients must be greater than 0.");
        }

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(Sender.Name, Sender.Address));
        foreach (var recipient in reсipients)
        {
            mimeMessage.From.Add(new MailboxAddress(recipient.Name, recipient.Address));
        }
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
        {
            Text = message.Body,
        };

        using (var client = new SmtpClient())
        {
            client.Connect(Sender.Host, Sender.Port, false);
            client.Authenticate(Sender.Login, Sender.Password);
            client.Send(mimeMessage);
            client.Disconnect(true);
        }
    }
}
