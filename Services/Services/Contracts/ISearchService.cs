using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;

namespace Services.Services.Contracts
{
    public interface ISearchService
    {
        Task<IEnumerable<BookGetVM>> GetBooksByPrompt(string prompt, CancellationToken cancellationToken);
        Task<IEnumerable<AuthorGetVM>> GetAuthorsPrompts(string prompt, CancellationToken cancellationToken);
        Task<IEnumerable<BookGetConciseVM>> GetBooksPrompts(string prompt, CancellationToken cancellationToken);
        IEnumerable<BookGetConciseVM> GetBooksSuggestions();
        IEnumerable<AuthorGetVM> GetAuthorsSuggestions();
    }
}
