using Microsoft.AspNetCore.Mvc;
using Services.Services.Contracts;
using Services.ViewModels.AuthVMs;
using Web.PageViewModels;

namespace Web.Controllers
{
    public class AuthController : BaseController
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

        [HttpPost]
        public async Task<IActionResult> Login([FromForm(Name = nameof(LoginPageVM.LoginPost))] LoginPostVM loginVM, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(new LoginPageVM(loginVM));
            }

            return Result(await _authService.Login(loginVM),
                () => RedirectToAction(nameof(HomeController.Index), "Home"),
                () => View(new LoginPageVM(loginVM)));
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout(LogoutPostVM logoutVM)
        {
            await _authService.Logout();

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

            return Result(await _authService.Register(registerVM),
                () => RedirectToAction(nameof(Login)),
                () => View(new RegisterPageVM(registerVM)));
        }
    }
}
