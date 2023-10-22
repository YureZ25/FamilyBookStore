using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.AuthVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginPageVM());
        }

        public async Task<IActionResult> Login([FromForm(Name = nameof(LoginPageVM.LoginPost))] LoginPostVM loginVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(new LoginPageVM(loginVM));
            }

            if (!await _authService.Login(loginVM, cancellationToken))
            {
                return View(new LoginPageVM(loginVM));
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterPageVM());
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm(Name = nameof(RegisterPageVM.RegisterPost))] RegisterPostVM registerVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(new RegisterPageVM(registerVM));
            }

            if (!await _authService.Register(registerVM, cancellationToken))
            {
                return View(new RegisterPageVM(registerVM));
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
