using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Services.Services.Contracts;
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

        public async Task<bool> Login(LoginPostVM loginVM, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user is null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(user, loginVM.Password);
        }

        public async Task<bool> Register(RegisterPostVM registerVM, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = registerVM.UserName,
            };

            var res = await _userManager.CreateAsync(user, registerVM.Password);

            return res.Succeeded;
        }
    }
}
