using Services.ViewModels;
using Services.ViewModels.AuthVMs;

namespace Services.Services.Contracts
{
    public interface IAuthService
    {
        Task<UserGetVM> GetCurrentUser();
        Task<ResultVM> Login(LoginPostVM loginVM);
        Task<ResultVM> Register(RegisterPostVM registerVM);
        Task Logout();
    }
}
