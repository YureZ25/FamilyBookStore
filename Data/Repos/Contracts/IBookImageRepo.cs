using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IBookImageRepo : IBaseRepo<BookImage>
    {
        Task<BookImage> GetByBookId(int bookId, CancellationToken cancellationToken);
    }
}
