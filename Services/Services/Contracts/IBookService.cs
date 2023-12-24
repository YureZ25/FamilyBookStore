using Data.Enums;
using Services.ViewModels;
using Services.ViewModels.BookVMs;

namespace Services.Services.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookGetVM>> GetUserBooksByStatusAsync(BookStatus bookStatus, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooksAsync(CancellationToken cancellationToken);
        Task<BookGetVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ResultVM<BookGetVM>> InsertAsync(BookPostVM bookVM, CancellationToken cancellationToken);
        Task<ResultVM<BookGetVM>> UpdateAsync(BookPostVM bookVM, CancellationToken cancellationToken);
        Task<ResultVM<BookGetVM>> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
