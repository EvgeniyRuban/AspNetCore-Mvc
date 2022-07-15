using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApp.Domain;

namespace WebApp.Services;

public sealed class SmtpEmailService : IEmailService
{
    private ILogger<SmtpEmailService> _logger;
    private MessageSenderInfo _sender = null!;

    public SmtpEmailService(IOptions<SmtpCredentials> options, ILogger<SmtpEmailService> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        try
        {
            Sender = new()
            {
                Name = options.Value.Name,
                Host = options.Value.Host,
                Login = options.Value.Login,
                Address = options.Value.Address,
                Password = options.Value.Password,
                Port = options.Value.Port,
            };
        }  
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public MessageSenderInfo Sender
    {
        get => _sender;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

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
    /// Sending a message recipient.
    /// </summary>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task SendMessageAsync(
        MailMessage message, 
        MessageRecipientInfo reсipient, 
        CancellationToken cancelToken = default)
    {
        ArgumentNullException.ThrowIfNull(reсipient);
        ArgumentNullException.ThrowIfNull(message);

        if (cancelToken.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(Sender.Name, Sender.Address));
        mimeMessage.To.Add(new MailboxAddress(reсipient.Name, reсipient.Address));
        mimeMessage.Subject = message.Subject;
        mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
        {
            Text = message.Body,
        };

        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(Sender.Host, Sender.Port, false);
                await client.AuthenticateAsync(Sender.Login, Sender.Password);
                await client.SendAsync(mimeMessage);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        if (cancelToken.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }
    }
}
