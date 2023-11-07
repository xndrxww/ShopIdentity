using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopIdentity.Models;

namespace ShopIdentity.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IIdentityServerInteractionService identityServerInteractionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _identityServerInteractionService = identityServerInteractionService;
        }

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
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) 
            {
                return View(loginViewModel);
            }

            var user = await _userManager.FindByNameAsync(loginViewModel.Username);
            if (user == null) 
            {
                ModelState.AddModelError(string.Empty, "Пользователь не найден");
                return View(loginViewModel);
            }

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Username,
                loginViewModel.Password, false, false);
            if (result.Succeeded) 
            {
                return Redirect(loginViewModel.ReturnUrl);
            }
            ModelState.AddModelError(string.Empty, "Ошибка аутентификации");

            return View(loginViewModel);
        }

        [HttpGet]
        public IActionResult Registration(string returnUrl)
        {
            var registrationViewModel = new RegistrationViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(registrationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel registrationViewModel)
        {
            if (!ModelState.IsValid) 
            {
                return View(registrationViewModel);
            }

            var user = new AppUser
            {
                UserName = registrationViewModel.Username
            };

            var result = await _userManager.CreateAsync(user, registrationViewModel.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction(registrationViewModel.ReturnUrl);
            }

            ModelState.AddModelError(string.Empty, "При регистрации произошла ошибка");
            return View(registrationViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            var logoutRequest = await _identityServerInteractionService.GetLogoutContextAsync(logoutId);
            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }
    }
}
