using Microsoft.AspNetCore.Mvc;
using WebApp.Domain;

namespace WebApp.Controllers;

public class ProductCatalogController : Controller
{
    private readonly IProductService _productService;
    private readonly IEmailService _emailService;

    public ProductCatalogController(IProductService productService, IEmailService emailService)
    {
        _productService = productService;
        _emailService = emailService;
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
    public async Task<IActionResult> ProductAddition([FromForm] ProductToCreate product, CancellationToken cancelToken = default)
    {
        await _productService.AddAsync(product, cancelToken);

        return View();
    }
}
