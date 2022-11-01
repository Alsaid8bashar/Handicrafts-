using handi_crafts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace handi_crafts.Controllers
{
    public class AdminBaseController : Controller
    {
        private readonly ModelContext _context;
        public AdminBaseController(ModelContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var id = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Userrs.Where(x => x.Id == id).AsNoTracking<Userr>().SingleOrDefault();
            ViewBag.AdminInfo = user;
        }
    }
}
