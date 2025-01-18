using Services.ViewModels;
using Services.ViewModels.BookQuoteVMs;

namespace Services.Services.Contracts
{
    public interface IBookQuoteService
    {
        Task<IEnumerable<BookQuoteGetVM>> GetByBook(int bookId, CancellationToken cancellationToken);
        Task<ResultVM<BookQuoteGetVM>> Insert(BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken);
    }
}
