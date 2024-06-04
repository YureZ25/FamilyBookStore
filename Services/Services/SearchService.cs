using Data.Entities;
using Data.Entities.Contracts;
using Data.Extensions;
using Data.Repos.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;
using Services.ViewModels.BookVMs;

namespace Services.Services
{
    internal class SearchService : ISearchService
    {
        private static readonly string _bookCacheKey = nameof(Book);
        private static readonly string _authorCacheKey = nameof(Author);
        private static readonly Func<ICacheEntry, Dictionary<string, SearchCacheEntity<Book>>> _bookCacheEntityFactory = (_) => [];
        private static readonly Func<ICacheEntry, Dictionary<string, SearchCacheEntity<Author>>> _authorCacheEntityFactory = (_) => [];

        private readonly IBookRepo _bookRepo;
        private readonly IAuthorRepo _authorRepo;
        private readonly IMemoryCache _memoryCache;

        public SearchService(IBookRepo bookRepo, IAuthorRepo authorRepo, IMemoryCache memoryCache)
        {
            _bookRepo = bookRepo;
            _authorRepo = authorRepo;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<BookGetVM>> GetBooksByPrompt(string prompt, CancellationToken cancellationToken)
        {
            var found = new List<BookGetVM>();
            foreach (var book in await _bookRepo.GetAll(cancellationToken))
            {
                var match = false;
                if (MatchPrompt(book.Title, prompt))
                {
                    var topBooks = _memoryCache.GetOrCreate(_bookCacheKey, _bookCacheEntityFactory);
                    var topBook = topBooks.GetOrCreate(book.GetCacheKey(), new() { Entity = book });
                    topBook.SearchCount++;
                    match = true;
                }
                if (MatchPrompt(book.Author.FullName, prompt))
                {
                    var topAuthors = _memoryCache.GetOrCreate(_authorCacheKey, _authorCacheEntityFactory);
                    var topAuthor = topAuthors.GetOrCreate(book.Author.GetCacheKey(), new() { Entity = book.Author });
                    topAuthor.SearchCount++;
                    match = true;
                }

                if (match) found.Add(book.Map());
            }
            return found;
        }

        public async Task<IEnumerable<BookGetConciseVM>> GetBooksPrompts(string prompt, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetAll(cancellationToken);

            return books
                .Where(e => MatchPrompt(e.Title, prompt))
                .Select(e => e.MapConcise());
        }

        public async Task<IEnumerable<AuthorGetVM>> GetAuthorsPrompts(string prompt, CancellationToken cancellationToken)
        {
            var authors = await _authorRepo.GetAll(cancellationToken);

            return authors
                .Where(e => MatchPrompt(e.FullName, prompt))
                .Select(e => e.Map());
        }

        public IEnumerable<BookGetConciseVM> GetBooksSuggestions()
        {
            var topAuthors = _memoryCache.GetOrCreate(_bookCacheKey, _bookCacheEntityFactory);

            return topAuthors.Values.OrderBy(e => e.SearchCount).Select(e => e.Entity.MapConcise());
        }

        public IEnumerable<AuthorGetVM> GetAuthorsSuggestions()
        {
            var topAuthors = _memoryCache.GetOrCreate(_authorCacheKey, _authorCacheEntityFactory);

            return topAuthors.Values.OrderBy(e => e.SearchCount).Select(e => e.Entity.Map());
        }

        private bool MatchPrompt(string input, string prompt)
        {
            return prompt.Split(' ')
                .Any(d => input.Contains(d, StringComparison.InvariantCultureIgnoreCase));
        }

        private class SearchCacheEntity<T> where T : class, ICommonEntity
        {
            public T Entity { get; set; }
            public int SearchCount { get; set; }
        }
    }
}
