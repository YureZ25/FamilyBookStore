using Services.ViewModels;
using Services.ViewModels.BookQuoteVMs;

namespace Services.Services.Contracts
{
    public interface IBookQuoteService
    {
        Task<IEnumerable<BookQuoteGetVM>> GetByBook(int bookId, CancellationToken cancellationToken);
        Task<BookQuoteGetVM> GetById(int id, CancellationToken cancellationToken);
        Task<ResultVM<BookQuoteGetVM>> Insert(BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken);
        Task<ResultVM<BookQuoteGetVM>> Update(BookQuotePostVM bookQuoteVM, CancellationToken cancellationToken);
        Task<ResultVM<BookQuoteGetVM>> DeleteById(int id, CancellationToken cancellationToken);
    }
}
