using handi_crafts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace handi_crafts.Controllers
{
    public class VendorController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public VendorController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;


        }


        public IActionResult Index()
        {
            var id2 = HttpContext.Session.GetInt32("VendorId");
            var user = _context.Userrs.Where(x => x.Id == id2).FirstOrDefault();
            var Cart = _context.Carts.Where(x => x.UserId == id2).FirstOrDefault();
            ViewBag.NumberOfProduct =_context.CartProducts.Include(p => p.Product).Where(x => x.CartId == Cart.Idd).Count();

            //var VendorCartProduct = _context.CartProducts.Where(x => x.CartId == Cart.Idd).ToList();
            
            //var soldProduct = _context.CartProducts.Where(x => x.Cart.Purchasestate == 1).ToList();
            //var sales = 0;
            //int count = 0;
            // foreach(var item in soldProduct)
            //{
            //    sales = soldProduct.Where(x => VendorCartProduct.Where(y => y.ProductId == x.ProductId).Count);

            //}

            


            ViewBag.VendorInfo = user;
            return View();
        }





        [HttpGet]
        public async Task<IActionResult> UpdateProfile(decimal? id)
        {


            var id2 = HttpContext.Session.GetInt32("VendorId"); // 81
            var user = _context.Userrs.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.VendorInfo = user;
            if (id == null)
            {
                return NotFound();
            }


            var userr = await _context.Userrs.FindAsync(id);
            ViewBag.Vendordata = _context.UserrLogins.Where(x => x.UserrId == id).SingleOrDefault();
            if (userr == null)
            {
                return NotFound();
            }
            return View(userr);
        }

        // POST: Userrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(decimal id, [Bind("Id,FirstName,LastName,ImagePath,ImageFile")] Userr userr, String password, String userName, String email)
        {
      

            if (id != userr.Id)
            {
                return NotFound();
            }



            if (ModelState.IsValid)
            {
                try
                {
                    if (userr.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        userr.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                        fileName);
                        using (var fileStream = new FileStream(path,
                        FileMode.Create))
                        {
                            await userr.ImageFile.CopyToAsync(fileStream);
                        }
                        userr.ImagePath = fileName;

                    }
                    else
                    {


                        userr.ImagePath = _context.Userrs.Where(x => x.Id == id).AsNoTracking<Userr>().SingleOrDefault().ImagePath;


                    }

                    _context.Update(userr);
                    await _context.SaveChangesAsync();
                    UserrLogin user = _context.UserrLogins.Where(x => x.UserrId == id).SingleOrDefault();

                    user.UserrName = userName;
                    user.Passwordd = password;
                    user.Email = email;
                    user.UserrId = id;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            return View(userr);
        }


        public async Task<IActionResult> Crafts()
        {

           
            var id = HttpContext.Session.GetInt32("VendorId");
            var user = _context.Userrs.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.VendorInfo = user;
            var Cart = _context.Carts.Where(x => x.UserId == id).FirstOrDefault() ;

            var modelContext = _context.CartProducts.Include(p => p.Product).Where(x=>x.CartId==Cart.Idd);
            List<Category> categories = new List<Category>();
            foreach (var item in modelContext)
            {
                categories.Add(_context.Categories.Where(x => x.Id == item.Product.CategoryId).SingleOrDefault());
            }



            ViewBag.category = categories;
          
            return View(await modelContext.ToListAsync());


        }
        public async Task<IActionResult> AddCrafts()
        {
            var id = HttpContext.Session.GetInt32("VendorId"); 

            var user = _context.Userrs.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.VendorInfo = user;
            ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCrafts([Bind("Id,Name,Price,CategoryId,Descriptionn,ImagePath,Quantity,ImageFile")] Product product)
        {

            var id = HttpContext.Session.GetInt32("VendorId"); // 81

            var user = _context.Userrs.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.VendorInfo = user;
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "" +
                    product.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(fileStream);
                    }
                    product.Imagepath = fileName;
                  
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
               
            }
       

            var userr = _context.Userrs.Where(x => x.Id == id).SingleOrDefault();
            var vendorCart = _context.Carts.Where(x => x.UserId == user.Id).FirstOrDefault();

            CartProduct cartProduct = new CartProduct();
            cartProduct.ProductId = product.Id;
            cartProduct.CartId = vendorCart.Idd;
            cartProduct.Quantity = 0;

            _context.Add(cartProduct);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Crafts));
        }




        public async Task<IActionResult> EditCrafts(decimal? id)
        {
            var id2 = HttpContext.Session.GetInt32("VendorId"); // 81

            var user = _context.Userrs.Where(x => x.Id == id2).FirstOrDefault();
            ViewBag.VendorInfo = user;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCrafts(decimal id, [Bind("Id,Name,Price,CategoryId,Descriptionn,Imagepath,Quantity,ImageFile")] Product product)
        {
      
            if (id != product.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        product.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                        fileName);
                        using (var fileStream = new FileStream(path,
                        FileMode.Create))
                        {
                            await product.ImageFile.CopyToAsync(fileStream);
                        }
                        product.Imagepath = fileName;

                    }
                    else
                    {
                     
                  
                        product.Imagepath = _context.Products.Where(x => x.Id == id).AsNoTracking<Product>().SingleOrDefault().Imagepath ;

                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction(nameof(Crafts));
            }
            ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View(product);


        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            var id2 = HttpContext.Session.GetInt32("VendorId"); // 81

            var user = _context.Userrs.Where(x => x.Id == id2).FirstOrDefault();
            ViewBag.VendorInfo = user;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(decimal id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Crafts));
        }

        public async Task<IActionResult> Details(decimal? id)
        {
            var id2 = HttpContext.Session.GetInt32("VendorId"); // 81

            var user = _context.Userrs.Where(x => x.Id == id2).FirstOrDefault();
            ViewBag.VendorInfo = user;
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }





        private bool ProductExists(decimal id)
        {
            return _context.Products.Any(e => e.Id == id);
        }





    }
}
