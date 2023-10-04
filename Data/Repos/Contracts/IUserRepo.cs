using Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Data.Repos.Contracts
{
    public interface IUserRepo : IUserStore<User>, IUserPasswordStore<User>
    {
        Task<User> FindByIdAsync(int userId, CancellationToken cancellationToken);
    }
}
