using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WebApp.Domain;

namespace WebApp.Services;

/// <summary>
/// The background service notifies the listener of its own availability, 
/// indicating the time since the start of the current session.
/// </summary>
public sealed class ServerStatusNotificationService : BackgroundService
{
    private readonly ILogger<ServerStatusNotificationService> _logger;
    private readonly IEmailSender _emailSender;
    private readonly ServerStatusNotificationConfig _config;

    public ServerStatusNotificationService(
        IEmailSender emailSender, 
        ILogger<ServerStatusNotificationService> logger,
        IOptions<ServerStatusNotificationConfig> config)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(emailSender);
        ArgumentNullException.ThrowIfNull(config);

        _logger = logger;
        _emailSender = emailSender;
        _config = config.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var message = new MailMessage
        {
            Subject = _config.MessageSubject,
            Body = _config.MessageBody,
        };
        var recipient = new MessageRecipientInfo
        {
            Address = _config.RecipientAddress,
            Name = _config.RecipientName,
        };

        var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        Stopwatch sw = Stopwatch.StartNew();

        while(await timer.WaitForNextTickAsync(stoppingToken))
        {
            
            await _emailSender.SendMessage(message, recipient, stoppingToken);
            _logger.LogInformation("{Message}. Current session length - {Time}", message.Body, sw.Elapsed);
        }
    }
}