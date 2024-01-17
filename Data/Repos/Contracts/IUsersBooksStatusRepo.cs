using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IUsersBooksStatusRepo : IBaseRepo<UsersBooksStatus>
    {
        Task<UsersBooksStatus> GetStatus(int userId, int bookId, CancellationToken cancellationToken);
    }
}
