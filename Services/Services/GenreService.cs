using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class GenreService : IGenreService
    {
        private readonly IGenreRepo _genreRepo;

        public GenreService(IGenreRepo genreRepo)
        {
            _genreRepo = genreRepo;
        }

        public async Task<IEnumerable<GenreVM>> GetGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreRepo.GetGenresAsync(cancellationToken);

            return genres.Select(e => e.Map());
        }
    }
}
