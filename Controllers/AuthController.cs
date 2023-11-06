using Microsoft.AspNetCore.Mvc;
using ShopIdentity.Models;

namespace ShopIdentity.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUtl)
        {
            var loginViewModel = new LoginViewModel
            {
                Password = returnUtl
            };
            return View(loginViewModel);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {
            return View(loginViewModel);
        }
    }
}
