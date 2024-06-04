using Services.ViewModels.AuthorVMs;

namespace Services.Services.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorGetVM>> GetAuthors(CancellationToken cancellationToken);
        Task<AuthorGetVM> GetById(int id, CancellationToken cancellationToken);
        Task<AuthorGetVM> Insert(AuthorPostVM authorVM, CancellationToken cancellationToken);
        Task<AuthorGetVM> Update(AuthorPostVM authorVM, CancellationToken cancellationToken);
        Task<AuthorGetVM> DeleteById(int id, CancellationToken cancellationToken);
    }
}
