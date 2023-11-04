using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.AuthVMs;

namespace Services.Services
{
    internal class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;

        public AuthService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResultVM> Login(LoginPostVM loginVM, CancellationToken cancellationToken)
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

            return new();
        }

        public async Task<ResultVM> Register(RegisterPostVM registerVM, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = registerVM.UserName,
            };

            var createResult = await _userManager.CreateAsync(user, registerVM.Password);

            return createResult.ToResultVM();
        }
    }
}
