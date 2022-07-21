using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class MetricsController : Controller
{
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(ILogger<MetricsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/metrics")]
    public IActionResult Metrics()
    {
        return View();
    }
}
