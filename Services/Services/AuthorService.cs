using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class AuthorService : IAuthorService
    {
        private readonly IAuthorRepo _authorRepo;

        public AuthorService(IAuthorRepo authorRepo)
        {
            _authorRepo = authorRepo;
        }

        public async Task<IEnumerable<AuthorVM>> GetAuthorsAsync(CancellationToken cancellationToken)
        {
            var authors = await _authorRepo.GetAuthorsAsync(cancellationToken);

            return authors.Select(e => e.Map());
        }
    }
}
