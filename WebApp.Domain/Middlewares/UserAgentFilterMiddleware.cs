using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApp.Domain;

public sealed class UserAgentFilterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserAgentFilterMiddleware> _logger;
    private readonly UserAgentFilterConfig _config;

    public UserAgentFilterMiddleware(
        RequestDelegate next,
        ILogger<UserAgentFilterMiddleware> logger,
        IOptions<UserAgentFilterConfig> config)
    {
        _next = next;
        _logger = logger;
        _config = config.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userAgent = context.Request.Headers.UserAgent;

        foreach (var validAgent in _config.ValidUserAgents)
        {
            if (userAgent.ToString().ToLower().Contains(validAgent))
            {
                _logger.LogInformation("User-agent {agent} is valid.", userAgent);
                await _next(context);
                return;
            }
        }

        _logger.LogInformation("User-agent {agent} is invalid.", userAgent);
        await context.Response.WriteAsync("Current web browser is out of service!");

        return;
    }
}