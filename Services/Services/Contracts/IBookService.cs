using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookVM>> GetBooksAsync(CancellationToken cancellationToken);
        Task<BookVM> GetById(int id, CancellationToken cancellationToken);
        Task<BookVM> InsertAsync(BookVM bookVM, CancellationToken cancellationToken);
        Task<BookVM> UpdateAsync(BookVM bookVM, CancellationToken cancellationToken);
        Task<BookVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
