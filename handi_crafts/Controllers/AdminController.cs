using handi_crafts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace handi_crafts.Controllers
{
    public class AdminController : AdminBaseController
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment) : base(context)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;

        }

        public IActionResult Index()
        {
                ViewBag.NumberOfUsers = _context.Userrs.Count();
                ViewBag.NumberOfCraftsmen = _context.UserrLogins.Where(x => x.RoleId == 2).Count();
                ViewBag.NumberOfProduct = _context.Products.Count();
                ViewBag.NumberOfsold = _context.CartProducts.Where(x=>x.Cart.Purchasestate==1).Count();
        
            var SalesCharts= _context.CartProducts.Include(x=>x.Cart).Where(x=>x.Cart.Purchasestate == 1).ToList();
        
            List<int> salesMonths = new List<int>();
            foreach(var item in SalesCharts)
            {
                salesMonths.Add(item.Cart.DatePurchase.Value.Month);
            }

            ViewBag.Months = salesMonths;
            return View();
        }
        public IActionResult Index1()
        {
          
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> UpdateProfile(decimal? id)
        {


        
            if (id == null)
            {
                return NotFound();
            }


            var userr = await _context.Userrs.FindAsync(id);
            ViewBag.Admindata = _context.UserrLogins.Where(x => x.UserrId == id).SingleOrDefault();
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

        // POST: Userrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> TestimonialRequest()
        {

            var TestimonialAproval = _context.Testimonials.Include(x => x.User).ToList();


            return View(TestimonialAproval);
        }
        public async Task<IActionResult> AcceptTestimonial(decimal id)
        {
            

            var user = _context.Testimonials.Include(x => x.User).Where(x => x.Id == id).SingleOrDefault();

            user.Status = 1;

            _context.Update(user);




            await _context.SaveChangesAsync();
            return RedirectToAction("TestimonialRequest");
        }
        public async Task<IActionResult> RejectTestimonial(decimal id)
        {
            var user = _context.Testimonials.Include(x => x.User).Where(x => x.Id == id).SingleOrDefault();

            user.Status = 2;

            _context.Update(user);




            await _context.SaveChangesAsync();
            return RedirectToAction("TestimonialRequest");
        }



        public async Task<IActionResult> VendorRequest()
        {

         
            var vendorsOnAproval = _context.UserrLogins.Include(x => x.Userr).ToList();

            ViewBag.vendorsOnAproval = vendorsOnAproval;
            return View();
        }
        public void SendEmail(string email, string name)
        {

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress("AVIATO", "aviatohandicrafts20@gmail.com");

            message.From.Add(from);
            MailboxAddress to = new MailboxAddress(name, email);
            message.To.Add(to);
            message.Subject = "AVIATO";






            BodyBuilder body = new BodyBuilder();
            body.HtmlBody = "<p> We have received your request to be vendor on our website  </p>" +
                "<p>This is a notification that serves as confirmation that your request has been approved</p>" +
                "<h2>Regards ," +
                "Bashar </h2>";

            message.Body = body.ToMessageBody();

            using (var clinte = new SmtpClient())
            {
                clinte.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                clinte.Authenticate("aviatohandicrafts20@gmail.com", "zpcaxqdfyidctmvq");


                clinte.Send(message);
                clinte.Disconnect(true);

            }

        }


        public async Task<IActionResult> AcceptVendor(decimal id)
        {
            Cart VendorCart = new Cart();



            var user = _context.UserrLogins.Include(x => x.Userr).Where(x => x.Id == id).SingleOrDefault();


            VendorCart.UserId = user.UserrId;
            VendorCart.Purchasestate = 0;
            user.RoleId = 2;
            SendEmail(user.Email, user.Userr.FirstName);
            _context.Update(user);
            _context.Add(VendorCart);



            await _context.SaveChangesAsync();
            return RedirectToAction("VendorRequest");
        }
        public async Task<IActionResult> RejectVendor(decimal id)
        {
            var user = _context.UserrLogins.Where(x => x.Id == id).SingleOrDefault();
            user.RoleId = 22;

            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("VendorRequest");
        }
        public async Task<IActionResult> SalesSearch()
        {
        

            var modelContext = _context.CartProducts.Include(p => p.Cart).Include(p => p.Product);
            var Names = _context.Userrs.Include(p => p.Carts).ToList();
            ViewBag.Names = Names;
            
            return View(await modelContext.ToListAsync());

        }
        [HttpPost]
        public async Task<IActionResult> SalesSearch(DateTime? startDate, DateTime? endDate)
        {

            var Names = _context.Userrs.Include(p => p.Carts).ToList();
            ViewBag.Names = Names;
            var result = _context.CartProducts.Include(p => p.Cart).Include(p => p.Product);
            if (startDate == null && endDate == null)
            {
                ViewBag.TotalQuantity = result.Sum(x => x.Quantity);

                return View(await result.ToListAsync());
            }
            else if (startDate == null && endDate != null)
            {
                var res = await result.Where(x => x.Cart.DatePurchase.Value.Date == endDate).ToListAsync();
                ViewBag.TotalQuantity = result.Where(x => x.Cart.DatePurchase.Value.Date == endDate).Sum(x => x.Quantity);
                return View(res);
            }

            else if (startDate != null && endDate == null)
            {
                var res = await result.Where(x => x.Cart.DatePurchase.Value.Date == startDate).ToListAsync();
                ViewBag.TotalQuantity = result.Where(x => x.Cart.DatePurchase.Value.Date == startDate).Sum(x => x.Quantity);
                return View(res);
            }
            else
            {
                var res = await result.Where(x => x.Cart.DatePurchase.Value.Date >= startDate && x.Cart.DatePurchase.Value.Date <= endDate).ToListAsync();
                ViewBag.TotalQuantity = result.Where(x => x.Cart.DatePurchase.Value.Date >= startDate && x.Cart.DatePurchase.Value.Date <= endDate).Sum(x => x.Quantity);
                return View(res);
            }


            return View();
        }

        public async Task<IActionResult> MonthlyReports()
        {

           

            var modelContext = _context.CartProducts.Include(p => p.Cart).Include(p => p.Product);
            ViewBag.TotalQuantity = modelContext.Sum(x => x.Quantity);
            return View(await modelContext.ToListAsync());



        }


        [HttpPost]
        public async Task<IActionResult> MonthlyReports(int? Month)
        {

            
            var result = _context.CartProducts.Include(p => p.Cart).Include(p => p.Product);

            if (Month != null)
            {
                var res = await result.Where(x => x.Cart.DatePurchase.Value.Month == Month).ToListAsync();
                ViewBag.TotalQuantity = result.Where(x => x.Cart.DatePurchase.Value.Month == Month).Sum(x => x.Quantity);

                return View(res);

            }





            return View(await result.ToListAsync());
        }


        public async Task<IActionResult> AnnualReports()
        {

           

            var modelContext = _context.CartProducts.Include(p => p.Cart).Include(p => p.Product);
            ViewBag.TotalQuantity = modelContext.Sum(x => x.Quantity);
            return View(await modelContext.ToListAsync());



        }

        [HttpPost]
        public async Task<IActionResult> AnnualReports(int? Year)
        {

            
            var result = _context.CartProducts.Include(p => p.Cart).Include(p => p.Product);



            if (Year != null)
            {

                var res = await result.Where(x => x.Cart.DatePurchase.Value.Year == Year).ToListAsync();
                ViewBag.TotalQuantity = result.Where(x => x.Cart.DatePurchase.Value.Year == Year).Sum(x => x.Quantity);
                return View(res);

            }





            return View(await result.ToListAsync());
        }






        public async Task<IActionResult> Categories()
        {


            
            return View(await _context.Categories.ToListAsync());


        }
        public async Task<IActionResult> AddCategory()
        {
           
            ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory([Bind("Id,CategoryName,ImagePath,ImageFile")] Category category)
        {

       
            if (ModelState.IsValid)
            {
                if (category.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "" +
                    category.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await category.ImageFile.CopyToAsync(fileStream);
                    }
                    category.ImagePath = fileName;

                }
                _context.Add(category);
                await _context.SaveChangesAsync();

            }




            return RedirectToAction(nameof(Categories));
        }




        public async Task<IActionResult> EditCategory(decimal? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");
            return View(category);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(decimal id, [Bind("Id,CategoryName,ImagePath,ImageFile")] Category category)
        {

            if (id != category.Id)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    if (category.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        category.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                        fileName);
                        using (var fileStream = new FileStream(path,
                        FileMode.Create))
                        {
                            await category.ImageFile.CopyToAsync(fileStream);
                        }
                        category.ImagePath = fileName;

                    }
                    else
                    {


                        category.ImagePath = _context.Categories.Where(x => x.Id == id).AsNoTracking<Category>().SingleOrDefault().ImagePath;

                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {

                }
                ViewData["Id"] = new SelectList(_context.Categories, "Id", "CategoryName");
                return RedirectToAction(nameof(Categories));
            }

            return View(category);


        }

        public async Task<IActionResult> Delete(decimal? id)
        {
          
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryDelete(decimal id)
        {
        
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Categories));
        }

        public async Task<IActionResult> CategoryDelete(decimal? id)
        {
            

            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        public async Task<IActionResult> ManageHome()
        {
            

            var images = _context.Images.Where(x => x.PageId == 1).ToList();
            ViewBag.HomePageImages = images;

            return View();

        }





        [HttpPost]
        public async Task<IActionResult> ManageHome(decimal id, [Bind("Id,PageId,ImagePath,ImageFile")] Image image)
        {
            

            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                        image.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                        fileName);
                        using (var fileStream = new FileStream(path,
                        FileMode.Create))
                        {
                            await image.ImageFile.CopyToAsync(fileStream);
                        }
                        image.ImagePath = fileName;

                    }
                    image.PageId = 1;

                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ManageHome));
            }

            return View();

        }











        public async Task<IActionResult> ManageAboutUs()
        {
            ViewBag.NumberOfUsers = _context.UserrLogins.Where(x => x.RoleId == 3).Count();
            ViewBag.NumberOfProduct = _context.Products.Count();
            ViewBag.NumberOfOrder = _context.Carts.Count();
            var Aboutus = _context.Pages.Include(x => x.Images).Include(x => x.Contents).Where(x => x.Id == 3);

            return View(Aboutus);

        }

        [HttpPost]
        public async Task<IActionResult> ManageAboutUsImages(decimal id, IFormFile ImageFile)
        {
            var id2 = HttpContext.Session.GetInt32("AdminId"); // 81
            var user = _context.Userrs.Where(x => x.Id == id2).FirstOrDefault();
            ViewBag.AdminInfo = user;
            Image Aboutus = _context.Images.Where(x => x.PageId == 3).SingleOrDefault();
            
            


            try
            {
                if (ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                    ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/",
                    fileName);
                    using (var fileStream = new FileStream(path,
                    FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    Aboutus.ImagePath = fileName;
                    _context.Update(Aboutus);
                    await _context.SaveChangesAsync();
                }
               

            
            }
            catch (DbUpdateConcurrencyException)
            {

                    throw;
                
            }
            return RedirectToAction(nameof(ManageAboutUs));
        }

         

    



        [HttpPost]
        public async Task<IActionResult> ManageAboutUs(decimal id,string contant)
        {
            
            var Aboutus = _context.Pages.Include(x => x.Images).Include(x => x.Contents).Where(x => x.Id == 3);
          
            Content contant2=new Content();
            contant2.Content1 = contant;
            contant2.Id = id;
            contant2.PageId = 3;

            try
            {
                //if (ImageFile != null)
                //{
                //    string wwwRootPath = _webHostEnviroment.WebRootPath;
                //    string fileName = Guid.NewGuid().ToString() + "_" +
                //    ImageFile.FileName;
                //    string path = Path.Combine(wwwRootPath + "/Images/",
                //    fileName);
                //    using (var fileStream = new FileStream(path,
                //    FileMode.Create))
                //    {
                //        await ImageFile.CopyToAsync(fileStream);
                //    }
                //    image.ImagePath = fileName;

                //}
        
                _context.Update(contant2);

                await _context.SaveChangesAsync(); }

            catch (DbUpdateConcurrencyException)
            {


                return RedirectToAction(nameof(ManageAboutUs));

            }
            return RedirectToAction(nameof(ManageAboutUs));



        }






        private bool ImageExists(decimal id)
        {
            return _context.Images.Any(e => e.Id == id);
        }


    }

}
