using Microsoft.AspNetCore.Mvc;

namespace handi_crafts.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
