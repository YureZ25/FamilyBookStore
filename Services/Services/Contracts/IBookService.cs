using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookVM>> GetBooksByStoreAsync(int storeId, CancellationToken cancellationToken);
        Task<IEnumerable<BookVM>> GetBooksAsync(CancellationToken cancellationToken);
        Task<BookVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<BookVM> InsertAsync(BookVM bookVM, CancellationToken cancellationToken);
        Task<BookVM> UpdateAsync(BookVM bookVM, CancellationToken cancellationToken);
        Task<BookVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
