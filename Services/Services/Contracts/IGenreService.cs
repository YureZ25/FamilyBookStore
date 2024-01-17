using Services.ViewModels.GenreVMs;

namespace Services.Services.Contracts
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreGetVM>> GetGenres(CancellationToken cancellationToken);
        Task<GenreGetVM> GetById(int id, CancellationToken cancellationToken);
        Task<GenreGetVM> Insert(GenrePostVM genreVM, CancellationToken cancellationToken);
        Task<GenreGetVM> Update(GenrePostVM genreVM, CancellationToken cancellationToken);
        Task<GenreGetVM> DeleteById(int id, CancellationToken cancellationToken);
    }
}
