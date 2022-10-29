using Microsoft.AspNetCore.Mvc;

namespace handi_crafts.Controllers
{
    public class Customer : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
