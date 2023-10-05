using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreVM>> GetGenresAsync(CancellationToken cancellationToken);
        Task<GenreVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<GenreVM> InsertAsync(GenreVM genreVM, CancellationToken cancellationToken);
        Task<GenreVM> UpdateAsync(GenreVM genreVM, CancellationToken cancellationToken);
        Task<GenreVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
