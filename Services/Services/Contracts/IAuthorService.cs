using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorVM>> GetAuthorsAsync(CancellationToken cancellationToken);
    }
}
