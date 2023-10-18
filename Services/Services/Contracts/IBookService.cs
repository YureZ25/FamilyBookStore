using Services.ViewModels.BookVMs;

namespace Services.Services.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookGetVM>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooksAsync(CancellationToken cancellationToken);
        Task<BookGetVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<BookGetVM> InsertAsync(BookPostVM bookVM, CancellationToken cancellationToken);
        Task<BookGetVM> UpdateAsync(BookPostVM bookVM, CancellationToken cancellationToken);
        Task<BookGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
