using Data.Entities;
using Data.Enums;

namespace Data.Repos.Contracts
{
    public interface IBookRepo : IBaseRepo<Book>
    {
        Task<IEnumerable<Book>> GetBooksByUserStatus(int userId, BookStatus bookStatus, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByGenre(int genreId, CancellationToken cancellationToken);
        Task<IEnumerable<Book>> GetBooksByStore(int storeId, CancellationToken cancellationToken);
        Task<bool> AttachedToStore(int bookId, int storeId, CancellationToken cancellationToken);
        void AttachToStore(Book book);
        void DetachFromStore(Book book);
    }
}
