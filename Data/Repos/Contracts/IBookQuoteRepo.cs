using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IBookQuoteRepo : IBaseRepo<BookQuote>
    {
        Task<IEnumerable<BookQuote>> GetByBookId(int bookId, CancellationToken cancellationToken);
    }
}
