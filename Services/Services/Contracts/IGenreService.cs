using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreVM>> GetGenresAsync(CancellationToken cancellationToken);
    }
}
