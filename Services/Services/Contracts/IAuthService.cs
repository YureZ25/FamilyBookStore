using Services.ViewModels.AuthVMs;

namespace Services.Services.Contracts
{
    public interface IAuthService
    {
        Task<bool> Login(LoginPostVM loginVM, CancellationToken cancellationToken);
        Task<bool> Register(RegisterPostVM registerVM, CancellationToken cancellationToken);
    }
}
