using Services.ViewModels.AuthorVMs;

namespace Services.Services.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorGetVM>> GetAuthorsAsync(CancellationToken cancellationToken);
        Task<AuthorGetVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<AuthorGetVM> InsertAsync(AuthorPostVM authorVM, CancellationToken cancellationToken);
        Task<AuthorGetVM> UpdateAsync(AuthorPostVM authorVM, CancellationToken cancellationToken);
        Task<AuthorGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
