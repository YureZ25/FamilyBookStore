using Data.Context.Contracts;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels.AuthorVMs;

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

        public async Task<IEnumerable<AuthorGetVM>> GetAuthorsAsync(CancellationToken cancellationToken)
        {
            var authors = await _authorRepo.GetAll(cancellationToken);

            return authors.Select(e => e.Map());
        }

        public async Task<AuthorGetVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _authorRepo.GetById(id, cancellationToken);

            return author.Map();
        }

        public async Task<AuthorGetVM> InsertAsync(AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            var author = authorVM.Map();

            _authorRepo.Insert(author);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return author.Map();
        }

        public async Task<AuthorGetVM> UpdateAsync(AuthorPostVM authorVM, CancellationToken cancellationToken)
        {
            var author = authorVM.Map();

            _authorRepo.Update(author);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return author.Map();
        }

        public async Task<AuthorGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var author = await _authorRepo.GetById(id, cancellationToken);

            _authorRepo.DeleteById(author.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return author.Map();
        }
    }
}
