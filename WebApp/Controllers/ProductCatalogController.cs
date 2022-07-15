using Microsoft.AspNetCore.Mvc;
using Polly;
using WebApp.Domain;

namespace WebApp.Controllers;

public class ProductCatalogController : Controller
{
    private readonly ILogger<ProductCatalogController> _logger;
    private readonly IEmailSender _emailService;
    private readonly IProductService _productService;
    private readonly MessageRecipientInfo _recipient = new ()
    {
        Name = "Study",
        Address = "csharptest478@gmail.com",
    };

    public ProductCatalogController(
        IProductService productService, 
        IEmailSender emailService, 
        ILogger<ProductCatalogController> logger)
    {
        ArgumentNullException.ThrowIfNull(productService);
        ArgumentNullException.ThrowIfNull(emailService);

        _productService = productService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> Products(CancellationToken cancelToken = default)
    {
        var products = await _productService.GetAllAsync(cancelToken);
        return View(products);
    }

    [HttpGet]
    public IActionResult ProductAddition() => View();

    [HttpPost]
    public async Task<IActionResult> ProductAddition(
        [FromForm] ProductToCreate product, 
        CancellationToken cancelToken = default)
    {
        await _productService.AddAsync(product, cancelToken);

        var message = new MailMessage
        {
            Subject = "Test",
            Body = $"Product added to catalog.\n" +
            $"Title: {product.Title}\n" +
            $"Image: {product.Image}\n"
        };

        var policy = Policy
        .Handle<Exception>()
        .RetryAsync(3, onRetry: (exception, retryAttempt) =>
        {
            _logger.LogWarning(
            exception, "Error while sending email. Retrying: {Attempt}", retryAttempt);
        });

        var result = await policy.ExecuteAndCaptureAsync(
            token => _emailService.SendMessageAsync(message, _recipient, token), cancelToken);

        if (result.Outcome == OutcomeType.Failure)
        {
            _logger.LogError(result.FinalException, "There was an error while sending email");
        }

        return View();
    }
}