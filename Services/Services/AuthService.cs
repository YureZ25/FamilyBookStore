using Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Exeptions;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.AuthVMs;
using Services.ViewModels.UserVMs;
using System.Security.Claims;

namespace Services.Services
{
    internal class AuthService : IAuthService, IUserClaimsPrincipalFactory<User>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserGetVM> GetCurrentUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new EntityNotFoundExeption(nameof(User), null);

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new EntityNotFoundExeption(nameof(User), userId);

            return user.Map();
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

            var principal = await CreateAsync(user);

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

        public Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.AuthenticationMethod, "pwd"),
            };

            var identity = new ClaimsIdentity(claims, "BasicAuth");

            var principal = new ClaimsPrincipal(identity);

            return Task.FromResult(principal);
        }
    }
}
