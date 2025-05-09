using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 
using MyApp.Models;             
using MyApp.Services;         
using System.Threading.Tasks; 


namespace MyApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseService _dbService;

        public AccountController(DatabaseService dbService)
        {
            _dbService = dbService;
            
        }


        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl=null)
        {
            if (ModelState.IsValid)
            {
                var userInfo = _dbService.GetUserLogin(model.Username!, model.Password!);

                if (userInfo != null)
                {
                    HttpContext.Session.SetString("UserId", userInfo.Username!);
                    return RedirectToAction("Index", "Home");
                }
            }
            if (!ModelState.IsValid || _dbService.GetUserLogin(model.Username!, model.Password!) == null)
            {
                if (ModelState.IsValid) 
                {
                    model.ErrorMessage = "Invalid login or password";
                }
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login", "Account");
        }


    }
}