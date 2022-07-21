using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace WebApp.Domain;

public sealed class PageHitCounterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PageHitCounterMiddleware> _logger;
    private Dictionary<string, int> _dictionary = new();

    public PageHitCounterMiddleware(RequestDelegate next, ILogger<PageHitCounterMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (_dictionary.ContainsKey(path))
        {
            _dictionary[path]++;
        }
        else
        {
            _dictionary.Add(path, 1);
        }

        _logger.LogCritical("Route '{path}' hitting {count}", path, _dictionary[path]);

        await _next(context);
    }
}