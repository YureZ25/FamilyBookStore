using Services.ViewModels.GenreVMs;

namespace Services.Services.Contracts
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreGetVM>> GetGenresAsync(CancellationToken cancellationToken);
        Task<GenreGetVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<GenreGetVM> InsertAsync(GenrePostVM genreVM, CancellationToken cancellationToken);
        Task<GenreGetVM> UpdateAsync(GenrePostVM genreVM, CancellationToken cancellationToken);
        Task<GenreGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
