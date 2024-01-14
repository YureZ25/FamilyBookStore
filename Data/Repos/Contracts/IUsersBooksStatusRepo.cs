using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IUsersBooksStatusRepo
    {
        Task<UsersBooksStatus> GetStatusAsync(int userId, int bookId, CancellationToken cancellationToken);
        void Insert(UsersBooksStatus usersBooksStatus);
        void Update(UsersBooksStatus usersBooksStatus);
        void DeleteById(int id);
    }
}
