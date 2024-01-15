using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IBookImageRepo
    {
        Task<BookImage> GetByBookId(int bookId, CancellationToken cancellationToken);
        Task<BookImage> GetById(int id, CancellationToken cancellationToken);
        void Insert(BookImage bookImage);
        void Update(BookImage bookImage);
        void DeleteById(int id);
    }
}
