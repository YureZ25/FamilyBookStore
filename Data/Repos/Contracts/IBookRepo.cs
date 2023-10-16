using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IBookRepo
    {
        Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken);
        Task<Book> GetByIdAsync(int id, CancellationToken cancellationToken);
        void AttachToStore(Book book);
        void DetachFromStore(Book book);
        void Insert(Book book);
        void Update(Book book);
        void DeleteById(int id);
    }
}
