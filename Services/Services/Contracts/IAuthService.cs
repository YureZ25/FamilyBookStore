using Services.ViewModels;
using Services.ViewModels.AuthVMs;

namespace Services.Services.Contracts
{
    public interface IAuthService
    {
        Task<ResultVM> Login(LoginPostVM loginVM, CancellationToken cancellationToken);
        Task<ResultVM> Register(RegisterPostVM registerVM, CancellationToken cancellationToken);
    }
}
