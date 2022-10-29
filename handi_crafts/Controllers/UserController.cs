using handi_crafts.Controllers.Helpers;
using handi_crafts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.IO;
using System.Linq;

using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MailKit.Security;
using System.Collections.Generic;

using IronPdf;
using System.Drawing;
namespace handi_crafts.Controllers
{
    public class UserController : BaseController1
    {
        // GET: UserController

        private readonly ILogger<UserController> _logger;
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public UserController(ModelContext context, IWebHostEnvironment webHostEnviroment, ILogger<UserController> logger) :base(context)
        {
            _logger = logger;
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
        public async Task<IActionResult> Shop(decimal ? id)
        {
          var  modelContext = _context.Products.Include(p => p.Category);
            if (id != null)
            {
            var categoryProducts = _context.Products.Include(p => p.Category).Where(x=>x.CategoryId==id).ToListAsync();
                return View(await categoryProducts);
            }
     

     
            return View(await modelContext.ToListAsync());

        }
        public async Task<IActionResult> Search(string catgeoryName)
        {
      
            
                var categoryProducts = _context.Products.Include(p => p.Category).Where(x=> x.Name.ToLower().Contains(catgeoryName.ToLower()));
              

            return View("shop",await categoryProducts.ToListAsync());

        }




        public async Task<IActionResult> AddToCart(decimal id,int ?Quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var Cart = _context.Carts.Where(x => x.UserId == userId).Where(x=>x.Purchasestate==0).SingleOrDefault();

            CartProduct Item = new CartProduct();
            Item.CartId = Cart.Idd;
            Item.ProductId = id;
     
            Item.Quantity = Quantity;
            _context.Add(Item);
            var product = _context.Products.Where(x => x.Id == id).SingleOrDefault();
        
       await _context.SaveChangesAsync();
            return RedirectToAction("Index", "User");

        }
        public async Task<IActionResult> SingleProduct(decimal id)
        {
            var product = _context.Products.Include(p => p.Category).Where(p => p.Id == id).SingleOrDefault();
            return View(product);

        }
        public async Task<IActionResult> RemoveFromCart(decimal? id)
        {
            
            var cartProduct = await _context.CartProducts.FindAsync(id);
            _context.CartProducts.Remove(cartProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "User");

        }
        public async Task<IActionResult> RemoveFromCart1(decimal? id)
        {
     
            var cartProduct = await _context.CartProducts.FindAsync(id);
            _context.CartProducts.Remove(cartProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction("Checkout", "User");

        }


        public async Task<IActionResult> AddToCart1(decimal id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var Cart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.Purchasestate == 0).SingleOrDefault();
 
            CartProduct Item = new CartProduct();
            Item.CartId = Cart.Idd;
            Item.ProductId = id;
            Item.Quantity = 1;

       
            _context.Add(Item);
            await _context.SaveChangesAsync();
            return RedirectToAction("Shop", "User");

        }






        public async Task<IActionResult> Cart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var Cart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.Purchasestate == 0).SingleOrDefault();

            var userCart = _context.CartProducts.Include(p => p.Product).Where(x => x.CartId == Cart.Idd).Where(x => x.Cart.Purchasestate == 0).ToList();

            return View(userCart);

        }
        [HttpGet]

            public async Task<IActionResult> Checkout()
            {
                var userId = HttpContext.Session.GetInt32("UserId");
       
                var check = _context.PaymentMethods.Include(p => p.Userr).Where(p=> p.UserrId==userId).FirstOrDefault();
                Cart UserCart = new Cart();

                UserCart.UserId = userId;
                UserCart.Purchasestate = 0;

                _context.Add(UserCart);
                if (check!=null)
                {
                    return View(check);
                }

                return View();

            }
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Checkout([Bind("Id,CardNumber,Balance,ExpiredDate,UserrId,Cvv,Name")] PaymentMethod paymentMethod, string FirstName, string AddressLine1, string City, string Country, string ZipCode)
            {

                var userId = HttpContext.Session.GetInt32("UserId");
                var user = _context.UserrLogins.Include(x=>x.Userr).Where(x => x.UserrId == userId).SingleOrDefault();
                  var Cart = _context.Carts.Where(x => x.UserId == userId).Where(x => x.Purchasestate == 0).SingleOrDefault(); ;
                var userCart = _context.CartProducts.Include(p => p.Product).Where(x => x.CartId == Cart.Idd).Where(x=> x.Cart.Purchasestate==0).ToList();
                paymentMethod.UserrId = userId;
          
                UserrAddress Address = new UserrAddress();
                Address.UserrId = userId;
                Address.FirstName = FirstName;
                Address.City = City;
                Address.ZipCode = ZipCode;
                Address.Country = Country;
                Address.AddressLine1 = AddressLine1;
                var check = _context.PaymentMethods.Include(p => p.Userr).Where(p => p.UserrId == userId).FirstOrDefault();
           
                if (check ==null)
                {
                    paymentMethod.Balance = 1000;
                }

                _context.Add(Address);
                _context.Add(paymentMethod);

                CreatePdf(userCart);
                var modelContext = _context.Products.ToList();
                int i = 0;
                int sum = 0;
                foreach (var item in userCart)
                {
                    _context.Products.Where(x => x.Id == userCart.ElementAt(i).Product.Id).SingleOrDefault().Quantity-=item.Quantity;
                    i++;
                    sum = (int)item.Quantity * (int)item.Product.Price;

                }
                paymentMethod.Balance-=sum;
                 Cart.Purchasestate = 1;
                Cart.DatePurchase= DateTime.Now;
                Cart UserCart = new Cart();

                UserCart.UserId = userId;
                UserCart.Purchasestate = 0;

                _context.Add(UserCart);
                 await _context.SaveChangesAsync();
                  
                  SendEmail(CreatePdf(userCart),user.Email,user.Userr.FirstName);
                return RedirectToAction(nameof(Confirmation));

            }
        public string CreatePdf(List<CartProduct> ProductsList)

        {


            //var pdfDocuments = new List<PdfDocument>();


            //foreach (var item in ProductsList)
            //{

            //    pdfDocuments.Add(PdfDocument.FromFile("Item Name" + item.Product.Name + " item Price: " + item.Product.Price));
            //}
            //var mergedPdfDocument = PdfDocument.Merge(pdfDocuments);
            //mergedPdfDocument.SaveAs("merged.pdf");


            //PdfDocument pdf = new PdfDocument();
            //PdfPage page = pdf.Pages.Add();
            //PdfGraphics graphic1 = page.Graphics;
            //PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            //graphic1.DrawString("Hello World!!!", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));

            //MemoryStream stream = new MemoryStream();

            //pdf.Save(stream);

            ////Set the position as '0'.
            //stream.Position = 0;

            ////Download the PDF document in the browser
            //FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");

            //fileStreamResult.FileDownloadName = "Sample.pdf";



            //< table class="table">
            //      <thead>
            //        <tr>
            //          <th class="">Item Name</th>
            //          <th class="">Item Price</th>
            //          <th class="">Actions</th>
            //        </tr>
            //      </thead>
            //      <tbody>
            //                   <tr class="">
            //    
            //          <td class="">$45</td>
            //          <td class="">
            //            <a class="product-remove" href="#!">Remove</a>
            //          </td>
            //        </tr>
           
     
            //      </tbody>
            //    </table>










            var userId = HttpContext.Session.GetInt32("UserId");

            string wwwRootPath = _webHostEnviroment.WebRootPath;
            var Renderer = new IronPdf.ChromePdfRenderer();
            string Items = "";
    
            foreach(var item in ProductsList)
            {
                Items  = Items+ "<td >"+ item.Product.Name + "</td>"  + "<td >" + item.Product.Price + "</td>";
                    
            }
            var PDF = Renderer.RenderHtmlAsPdf("< table > <thead>  <tr>  <th>Item Name</th> <th>Item Price</th>   </tr></thead><tbody> <tr>" + Items+ " </tr> </tbody> </table>");
            PDF.SaveAs("AVIATO"+userId+ ".pdf");
            return "AVIATO" + userId+".pdf";
         
        }


        public void SendEmail(string path ,string email,string name)
        {

            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress("AVIATO", "aviatohandicrafts20@gmail.com");

            message.From.Add(from);
            MailboxAddress to= new MailboxAddress(name, email);
            message.To.Add(to);
            message.Subject = "Your order from AVIATO";
           
            BodyBuilder body = new BodyBuilder();
            body.HtmlBody = "<h1>Your purches report</h1>";
             body.Attachments.Add(path);
            message.Body = body.ToMessageBody();
            
            using (var clinte = new SmtpClient())
            {
                clinte.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                clinte.Authenticate("aviatohandicrafts20@gmail.com", "zpcaxqdfyidctmvq");
              
             
                clinte.Send(message);
                clinte.Disconnect(true);

            }
              
        }





        public ActionResult Confirmation()
        {
         
            return View();
        }


        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
       
            var userProfile = _context.Userrs.Include(x=>x.UserrLogins).Include(x=> x.Carts).Where(x=>x.Id==userId).SingleOrDefault();

            return View(userProfile);

        }
        public async Task<IActionResult> Testimonils()
        {
          
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Testimonils([Bind("Content")] Testimonial testimonial)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            testimonial.Status = 0;
            testimonial.UserId=userId;
            _context.Add(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> Orders()
        { 
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.userid = userId;
           
            var userOrder = _context.CartProducts.Include(x => x.Cart).Include(x=>x.Product).Where(x => x.Cart.UserId == userId).ToList();

            return View(userOrder);

        }
        public async Task<IActionResult> Address()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.userid = userId;
            var userAdress = _context.UserrAddresses.Where(x=>x.UserrId==userId);

            return View(userAdress);

        }




        [HttpGet]
        public async Task<IActionResult> UpdateProfile(decimal? id)
        {


            var userId = HttpContext.Session.GetInt32("UserId");
            var Cart = _context.Carts.Where(x => x.UserId == userId).FirstOrDefault();
            var userCart = _context.CartProducts.Include(p => p.Product).Where(x => x.CartId == Cart.Idd).ToList();
            ViewBag.usercart = userCart;
            if (id == null)
            {
                return NotFound();
            }


            var userr = await _context.Userrs.FindAsync(id);
            ViewBag.UserData = _context.UserrLogins.Where(x => x.UserrId == id).SingleOrDefault();
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
                return RedirectToAction(nameof(Profile));
            }
            return View(userr);
        }
        public async Task<IActionResult> AboutUs()
        {
            
            ViewBag.NumberOfUsers = _context.UserrLogins.Where(x => x.RoleId == 3).Count();
            ViewBag.NumberOfProduct = _context.Products.Count();
            ViewBag.NumberOfOrder = _context.Carts.Count();
            var Aboutus = _context.Pages.Include(x => x.Images).Include(x => x.Contents).Where(x => x.Id == 3 );

            return View(Aboutus);
        }

        public async Task<IActionResult> ContactUs()
        {


         
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ContactUs(string name, string email ,string subject ,string message1)
        {


            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(name, email);

            message.From.Add(from);
            MailboxAddress to = new MailboxAddress("AVIATO", "aviatohandicrafts20@gmail.com");
            message.To.Add(to);
            message.Subject = subject;

            BodyBuilder body = new BodyBuilder();
            body.HtmlBody = message1;
            message.Body = body.ToMessageBody();

            using (var clinte = new SmtpClient())
            {
                clinte.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                clinte.Authenticate("aviatohandicrafts20@gmail.com", "zpcaxqdfyidctmvq");


                clinte.Send(message);
                clinte.Disconnect(true);

            }

            return View();
        }









        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
