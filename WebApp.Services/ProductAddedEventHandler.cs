using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using WebApp.Domain;

namespace WebApp.Services;

public class ProductAddedEventHandler : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ProductAddedEventHandler> _logger;
    private readonly ProductAddedEventHandlerConfig _config;
    private CancellationToken _stoppingToken;

    public ProductAddedEventHandler(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ProductAddedEventHandler> logger,
        IOptions<ProductAddedEventHandlerConfig> config)
    {
        _config = config.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        DomainEventsManager.Register<ProductAdded>(ev => { _ = SendEmailNotification(ev); });
    }

    private async Task SendEmailNotification(ProductAdded ev)
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var message = new MailMessage
        {
            Body = _config.MessageBody,
            Subject = _config.MessageSubject,
        };
        var recipient = new MessageRecipientInfo
        {
            Address = _config.RecipientAddress,
            Name = _config.RecipientName
        };

        Task SendAsync(CancellationToken cancellationToken)
        {
            return emailSender.SendMessage(message, recipient, cancellationToken: cancellationToken);
        }

        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(retryAttempt, 2)),
                (exception, retryAttempt) =>
                {
                    _logger.LogWarning(
                        exception, "There was an error while sending email. Retrying: {Attempt}", retryAttempt);
                });
        var result = await policy.ExecuteAndCaptureAsync(SendAsync, _stoppingToken);
        if (result.Outcome == OutcomeType.Failure)
            _logger.LogError(result.FinalException, "There was an error while sending email");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        return Task.CompletedTask;
    }
}
