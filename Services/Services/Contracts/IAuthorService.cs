using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorVM>> GetAuthorsAsync(CancellationToken cancellationToken);
        Task<AuthorVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<AuthorVM> InsertAsync(AuthorVM authorVM, CancellationToken cancellationToken);
        Task<AuthorVM> UpdateAsync(AuthorVM authorVM, CancellationToken cancellationToken);
        Task<AuthorVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
