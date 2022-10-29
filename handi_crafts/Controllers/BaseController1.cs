using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using handi_crafts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
namespace handi_crafts.Controllers
{
    public class BaseController1 : Controller
    {
        private readonly ModelContext _context;
        public BaseController1(ModelContext context)
        {
            _context = context;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
                {
                //User ID

                var userId = HttpContext.Session.GetInt32("UserId");
                var Cart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.Purchasestate == 0).SingleOrDefault();

                var userCart = _context.CartProducts.Include(p => p.Product).Where(x => x.CartId == Cart.Idd).Where(x => x.Cart.Purchasestate == 0).ToList();
                ViewBag.usercart = userCart;
           
                }


            }
        
    }
}

