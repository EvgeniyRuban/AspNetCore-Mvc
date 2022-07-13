using Microsoft.AspNetCore.Mvc;
using WebApp.Domain;

namespace WebApp.Controllers;

public class ProductCatalogController : Controller
{
    private readonly IProductService _productService;
    private readonly IEmailService _emailService;

    public ProductCatalogController(IProductService productService, IEmailService emailService)
    {
        ArgumentNullException.ThrowIfNull(productService);
        ArgumentNullException.ThrowIfNull(emailService);

        _productService = productService;
        _emailService = emailService;
        _emailService.Sender = new MessageSenderInfo
        {
            Name = "asp2022gb@rodion-m.ru",
            Host = "smtp.beget.com",
            Login = "asp2022gb@rodion-m.ru",
            Address = "asp2022gb@rodion-m.ru",
            Password = "3drtLSa1",
            Port = 25,
        };
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
        var message = new MailMessage
        {
            Subject = "Test",
            Body = $"Product added to catalog.\n" +
            $"Title: {product.Title}\n" +
            $"Image: {product.Image}\n"
        };
        var recipient = new MessageRecipientInfo
        {
            Name = "Study",
            Address = "csharptest478@gmail.com",
        };

        _emailService.SendMessage(message, recipient);

        return View();
    }
}
