using Data.Entities;
using Data.Enums;

namespace Data.Repos.Contracts
{
    public interface IBookRepo
    {
        Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByUserStatusAsync(int userId, BookStatus bookStatus, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken);
        Task<Book> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<bool> AttachedToStore(int bookId, int storeId, CancellationToken cancellationToken);
        void AttachToStore(Book book);
        void DetachFromStore(Book book);
        void Insert(Book book);
        void Update(Book book);
        void DeleteById(int id);
    }
}
