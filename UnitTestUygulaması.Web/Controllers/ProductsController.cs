using Microsoft.AspNetCore.Mvc;

namespace UnitTestUygulaması.Web.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
