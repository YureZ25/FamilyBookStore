using Data.Context.Contracts;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class AuthorService : IAuthorService
    {
        private readonly IAuthorRepo _authorRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IAuthorRepo authorRepo, IUnitOfWork unitOfWork)
        {
            _authorRepo = authorRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AuthorVM>> GetAuthorsAsync(CancellationToken cancellationToken)
        {
            var authors = await _authorRepo.GetAuthorsAsync(cancellationToken);

            return authors.Select(e => e.Map());
        }

        public async Task<AuthorVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _authorRepo.GetByIdAsync(id, cancellationToken);

            return author.Map();
        }

        public async Task<AuthorVM> InsertAsync(AuthorVM authorVM, CancellationToken cancellationToken)
        {
            var author = authorVM.Map();

            _authorRepo.Insert(author);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return author.Map();
        }

        public async Task<AuthorVM> UpdateAsync(AuthorVM authorVM, CancellationToken cancellationToken)
        {
            var author = authorVM.Map();

            _authorRepo.Update(author);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return author.Map();
        }

        public async Task<AuthorVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var author = await GetByIdAsync(id, cancellationToken);

            _authorRepo.DeleteById(author.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return author;
        }
    }
}
