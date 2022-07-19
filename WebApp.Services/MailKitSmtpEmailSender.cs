using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using WebApp.Domain;

namespace WebApp.Services;

/// <summary>
/// <typeparamref name = "MailKit"/> package based class which provides email sending by SMTP.
/// </summary>
public sealed class MailKitSmtpEmailSender : IEmailSender, IDisposable, IAsyncDisposable
{
    private readonly SmtpClient _smtpClient = new ();
    private readonly ILogger<MailKitSmtpEmailSender> _logger;
    private readonly MessageSenderInfo _sender = null!;

    public MailKitSmtpEmailSender(
        IOptions<SmtpConfig> config,
        ILogger<MailKitSmtpEmailSender> logger)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(logger);

        _sender = new MessageSenderInfo
        {
            Address = config.Value.Address,
            Login = config.Value.Login,
            Name = config.Value.Name,
            Password = config.Value.Password,
            Host = config.Value.Host,
            Port = config.Value.Port,
        };
        _logger = logger;

        if (string.IsNullOrEmpty(_sender.Address))
            throw new ArgumentNullException(nameof(_sender.Address), "Property value is null or empty!");
        if(string.IsNullOrEmpty(_sender.Login))
            throw new ArgumentNullException(nameof(_sender.Login), "Property value is null or empty!");
        if (string.IsNullOrEmpty(_sender.Name))
            throw new ArgumentNullException(nameof(_sender.Name), "Property value is null or empty!");
        if (string.IsNullOrEmpty(_sender.Password))
            throw new ArgumentNullException(nameof(_sender.Password), "Property value is null or empty!");
        if (string.IsNullOrEmpty(_sender.Host))
            throw new ArgumentNullException(nameof(_sender.Host), "Property value is null or empty!");
    }

    public async Task SendMessage(
        MailMessage message, 
        MessageRecipientInfo reсipient, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reсipient);
        ArgumentNullException.ThrowIfNull(message);

        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_sender.Name, _sender.Address));
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
    public void Dispose()
    {
        if (_smtpClient.IsConnected)
        {
            _smtpClient.Disconnect(true);
        }
        _smtpClient.Dispose();
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
            await _smtpClient.ConnectAsync(_sender.Host, _sender.Port, false, cancellationToken);
        }
    }
    private async Task EnsureAuthetication(CancellationToken cancellationToken = default)
    {
        if (!_smtpClient.IsAuthenticated)
        {
            await _smtpClient.AuthenticateAsync(_sender.Login, _sender.Password, cancellationToken);
        }
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