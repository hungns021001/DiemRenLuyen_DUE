using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DiemNgoaiKhoa.Helpers;
using DiemNgoaiKhoa.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiemNgoaiKhoa.Controllers
{
    
    public class AuthenController : Controller
    {
        private readonly ILogger<AuthenController> _logger;
        private readonly DataContext _context;
        public AuthenController(ILogger<AuthenController> logger,DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> Login(Login login) 
        {
            if(ModelState.IsValid)
            {
                //string PasswordHash = Helper.Hash128(login.Password);
                var user = await _context.Accounts.FirstOrDefaultAsync(x => x.Username == login.Username && x.Password == login.Password);
                if(user == null){
                    return Json(new {
                        Success = false,
                        Data = "Username or Password is incorrect!"
                    });
                }
                var role = _context.Role.Where(x => x.Id == user.RoleId).FirstOrDefault();
                if(role == null){
                    return Json(new {
                        Success = false,
                        Data = "Role is incorrect!"
                    });
                }
                await HttpContext.SignInAsync(user,role);
                return Json(new {
                    Success = true,
                    Data = "Login Success!"
                });

            }
            else
            {
                return Json(new {token = ""});
            }
        }
        // Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            if(User.Identity.IsAuthenticated){
              // Remove all cookies
                foreach (var cookie in Request.Cookies.Keys)
                {
                    Response.Cookies.Delete(cookie);
                }
            }
            return RedirectToAction("Index","Authen");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}