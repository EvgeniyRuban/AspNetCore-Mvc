using Microsoft.AspNetCore.Mvc;
using WebApp_MVC.Models;

namespace WebApp_MVC.Controllers
{
    public class CatalogController : Controller
    {
        private readonly Catalog _catalog;

        public CatalogController(Catalog catalog)
        {
            _catalog = catalog;
        }

        [HttpGet]
        public IActionResult Products() => View(_catalog.Items);

        [HttpGet]
        public IActionResult ProductAddition() => View();

        [HttpPost]
        public IActionResult ProductAddition([FromForm]Product item)
        {
            _catalog.Add(item);
            return View();
        }
    }
}
