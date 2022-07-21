using Microsoft.AspNetCore.Mvc;
using WebApp.Domain;

namespace WebApp.Controllers;

public class ProductCatalogController : Controller
{
    private readonly ILogger<ProductCatalogController> _logger;
    private readonly IProductService _productService;

    public ProductCatalogController(
        IProductService productService,
        ILogger<ProductCatalogController> logger)
    {
        ArgumentNullException.ThrowIfNull(productService);
        ArgumentNullException.ThrowIfNull(logger);

        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> Products(CancellationToken cancelToken = default)
    {
        var products = await _productService.GetAll(cancelToken);
        return View(products);
    }

    [HttpGet]
    public IActionResult ProductAddition() => View();

    [HttpPost]
    public async Task<IActionResult> ProductAddition(
        [FromForm] ProductToCreate product, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _productService.Add(product, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation was cancelled.");
            return View();
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogInformation(ex, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return View();
    }
}