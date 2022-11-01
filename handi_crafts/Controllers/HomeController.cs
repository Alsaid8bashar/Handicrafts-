using handi_crafts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace handi_crafts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HomeController(ModelContext context, IWebHostEnvironment webHostEnviroment) 
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;

        }
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Products.Include(p => p.Category);
            var Testimonil = _context.Testimonials.Include(x => x.User).Where(x => x.Status == 1).ToList();
            ViewBag.Testimonials = Testimonil;
            var images = _context.Images.Where(x => x.PageId == 1).ToList();
            ViewBag.HomePageImages = images;

            return View(await modelContext.ToListAsync());
        }
        public async Task<IActionResult> Shop(decimal? id)
        {
            var modelContext = _context.Products.Include(p => p.Category);
            if (id != null)
            {
                var categoryProducts = _context.Products.Include(p => p.Category).Where(x => x.CategoryId == id).ToListAsync();
                return View(await categoryProducts);
            }



            return View(await modelContext.ToListAsync());

        }
        public async Task<IActionResult> Search(string catgeoryName)
        {


            var categoryProducts = _context.Products.Include(p => p.Category).Where(x => x.Name.ToLower().Contains(catgeoryName.ToLower()));


            return View("shop", await categoryProducts.ToListAsync());

        }
        public async Task<IActionResult> AboutUs()
        {

            ViewBag.NumberOfUsers = _context.UserrLogins.Where(x => x.RoleId == 3).Count();
            ViewBag.NumberOfProduct = _context.Products.Count();
            ViewBag.NumberOfOrder = _context.Carts.Count();
            var Aboutus = _context.Pages.Include(x => x.Images).Include(x => x.Contents).Where(x => x.Id == 3);

            return View(Aboutus);
        }

        public async Task<IActionResult> ContactUs()
        {
                


            return View();
        }


    }
}
