using Data.Entities;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;

namespace Services.Services
{
    internal class SearchService : ISearchService
    {
        private readonly IBookRepo _bookRepo;
        private readonly IAuthorRepo _authorRepo;

        public SearchService(IBookRepo bookRepo, IAuthorRepo authorRepo)
        {
            _bookRepo = bookRepo;
            _authorRepo = authorRepo;
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksByPrompt(string prompt, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetAll(cancellationToken);

            return books
                .Where(e => MatchSearchPrompt(e, prompt))
                .Select(e => e.Map());
        }

        public async Task<IEnumerable<BookGetConciseVM>> GetBooksPrompts(string prompt, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetAll(cancellationToken);

            return books
                .Where(e => MatchSuggestionPrompt(e.Title, prompt))
                .Select(e => new BookGetConciseVM { Id = e.Id, Title = e.Title });
        }

        public async Task<IEnumerable<AuthorGetVM>> GetAuthorsPrompts(string prompt, CancellationToken cancellationToken)
        {
            var authors = await _authorRepo.GetAll(cancellationToken);

            return authors
                .Where(e => MatchSuggestionPrompt(e.FullName, prompt))
                .Select(e => e.Map());
        }

        private bool MatchSearchPrompt(Book book, string prompt)
        {
            foreach (string datum in prompt.Split(' '))
            {
                if (book.Title.Contains(datum, StringComparison.CurrentCultureIgnoreCase)) return true;

                if (book.Author.FullName.Contains(datum, StringComparison.InvariantCultureIgnoreCase)) return true;
            }

            return false;
        }

        private bool MatchSuggestionPrompt(string input, string prompt)
        {
            return prompt.Split(' ')
                .Any(d => input.Contains(d, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
