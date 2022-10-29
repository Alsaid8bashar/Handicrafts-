using handi_crafts.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace handi_crafts.Controllers
{
    public class LoginandRegistrationController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LoginandRegistrationController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login([Bind("UserrName , Passwordd")] UserrLogin userLogin)
        {
            var user = _context.UserrLogins.Where(x => x.UserrName == userLogin.UserrName && x.Passwordd == userLogin.Passwordd).SingleOrDefault();
           

            if (user != null)
            {
                switch (user.RoleId)
                {
                    case 1: // Admin
                        HttpContext.Session.SetInt32("AdminId", (int)user.UserrId);
                      
                        return RedirectToAction("Index", "Admin");
                    case 2: //Vendor
                        HttpContext.Session.SetInt32("VendorId", (int)user.UserrId);
                  

                        return RedirectToAction("Index", "Vendor");
                    case 3:
                        { //user

                            HttpContext.Session.SetInt32("UserId", (int)user.UserrId);
                            return RedirectToAction("Index", "User");
                        }
                }
            }
            ModelState.AddModelError("", "incorrect user name or password");

            return View();
        
    }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult VendorConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Id,FirstName,LastName,ImagePath,ImageFile")] Userr user1, string username, string password,string email) // user => fname , lname , imagefile
        {

            if (ModelState.IsValid)
            {

                    if (user1.ImageFile != null)
                    {
                        string wwwrootPath = _webHostEnvironment.WebRootPath; // wwwrootpath
                        string imageName = Guid.NewGuid().ToString() + "_" + user1.ImageFile.FileName; // image name
                        string path = Path.Combine(wwwrootPath + "/Images/", imageName); // wwwroot/Image/imagename

                        using (var filestream = new FileStream(path, FileMode.Create))
                        {
                            await user1.ImageFile.CopyToAsync(filestream);
                        }
                    user1.ImagePath = imageName;
                       
                  



                    }
                _context.Add(user1);
                await _context.SaveChangesAsync();
                UserrLogin user = new UserrLogin();
                user.UserrName = username;
                user.Passwordd = password;
                user.Email = email; 
                user.RoleId = 3;
                user.UserrId = user1.Id;


                _context.Add(user);
                Cart UserCart = new Cart();

                UserCart.UserId = user.UserrId;
                UserCart.Purchasestate = 0;

                _context.Add(UserCart);
                await _context.SaveChangesAsync();



         

                return RedirectToAction("Login");
            }

            return View(user1);
        }
        public IActionResult RegisterAsVendor()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterAsVendor([Bind("Id,FirstName,LastName,ImagePath,ImageFile")] Userr user1, string username, string password, string email) // user => fname , lname , imagefile
        {

            if (ModelState.IsValid)
            {

                if (user1.ImageFile != null)
                {
                    string wwwrootPath = _webHostEnvironment.WebRootPath; // wwwrootpath
                    string imageName = Guid.NewGuid().ToString() + "_" + user1.ImageFile.FileName; // image name
                    string path = Path.Combine(wwwrootPath + "/Images/", imageName); // wwwroot/Image/imagename

                    using (var filestream = new FileStream(path, FileMode.Create))
                    {
                        await user1.ImageFile.CopyToAsync(filestream);
                    }
                    user1.ImagePath = imageName;
                    _context.Add(user1);
                    await _context.SaveChangesAsync();
                    UserrLogin user = new UserrLogin();
                    user.UserrName = username;
                    user.Passwordd = password;
                    user.Email = email;
                    user.RoleId = 21;
                    user.UserrId = user1.Id;

                    _context.Add(user);
                    await _context.SaveChangesAsync();



                }

                return RedirectToAction("VendorConfirmation");
            }

            return View(user1);
        }


    }
}
