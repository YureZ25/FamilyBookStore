using Data.Enums;
using Services.ViewModels;
using Services.ViewModels.BookVMs;

namespace Services.Services.Contracts
{
    public interface IBookService
    {
        Task<IEnumerable<BookGetVM>> GetBooksByPrompt(string prompt, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetUserBooksByStatus(BookStatus bookStatus, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooksByStore(int storeId, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooksByAuthor(int authorId, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooksByGenre(int genreId, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetVM>> GetBooks(CancellationToken cancellationToken);
        Task<BookGetVM> GetById(int id, CancellationToken cancellationToken);
        Task<ResultVM<BookGetVM>> Insert(BookPostVM bookVM, CancellationToken cancellationToken);
        Task<ResultVM<BookGetVM>> Update(BookPostVM bookVM, CancellationToken cancellationToken);
        Task<ResultVM<BookGetVM>> DeleteById(int id, CancellationToken cancellationToken);
    }
}
