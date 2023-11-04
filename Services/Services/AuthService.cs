using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.AuthVMs;
using System.Security.Claims;

namespace Services.Services
{
    internal class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultVM> Login(LoginPostVM loginVM)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user is null)
            {
                return new("LoginPost.UserName", "Пользователь с таким никнеймом не найден");
            }

            if (!await _userManager.CheckPasswordAsync(user, loginVM.Password))
            {
                return new("LoginPost.Password", "Неправильный пароль");
            }

            var principal = CreatePrincipal(user);

            await _httpContextAccessor.HttpContext.SignInAsync(principal);

            return new();
        }

        public async Task<ResultVM> Register(RegisterPostVM registerVM)
        {
            var user = new User
            {
                UserName = registerVM.UserName,
            };

            var createResult = await _userManager.CreateAsync(user, registerVM.Password);

            return createResult.ToResultVM();
        }

        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }

        private ClaimsPrincipal CreatePrincipal(User user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, "pwd"),
            };

            var identity = new ClaimsIdentity(claims, "BasicAuth");

            return new ClaimsPrincipal(identity);
        }
    }
}
