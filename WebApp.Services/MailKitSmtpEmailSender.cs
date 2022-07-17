using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApp.Domain;

namespace WebApp.Services;

public sealed class MailKitSmtpEmailSender : IEmailSender, IDisposable, IAsyncDisposable
{
    private readonly SmtpClient _smtpClient = new ();
    private ILogger<MailKitSmtpEmailSender> _logger;
    private MessageSenderInfo _sender = null!;

    public MailKitSmtpEmailSender(
        IOptions<SmtpCredentials> options, 
        ILogger<MailKitSmtpEmailSender> logger)
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
    /// Sending a message to the recipient.
    /// </summary>
    /// <exception cref="TaskCanceledException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task SendMessage(
        MailMessage message, 
        MessageRecipientInfo reсipient, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reсipient);
        ArgumentNullException.ThrowIfNull(message);

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
            await EnsureConnectionAndAuthetication(cancellationToken);
            await _smtpClient.SendAsync(mimeMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
    private async Task EnsureConnectionAndAuthetication(CancellationToken cancellationToken = default)
    {
        await EnsureConnection(cancellationToken);
        await EnsureAuthetication(cancellationToken);
    }
    private async Task EnsureConnection(CancellationToken cancellationToken = default)
    {
        if (!_smtpClient.IsConnected)
        {
            await _smtpClient.ConnectAsync(Sender.Host, Sender.Port, false, cancellationToken);
        }
    }
    private async Task EnsureAuthetication(CancellationToken cancellationToken = default)
    {
        if (!_smtpClient.IsAuthenticated)
        {
            await _smtpClient.AuthenticateAsync(Sender.Login, Sender.Password, cancellationToken);
        }
    }
    public void Dispose()
    {
        if (_smtpClient.IsConnected)
        {
            _smtpClient.Disconnect(true);
        }
        _smtpClient.Dispose();
    }
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_smtpClient.IsConnected)
        {
            await _smtpClient.DisconnectAsync(true);
        }
        _smtpClient.Dispose();
    }
}
