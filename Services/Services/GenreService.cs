using Data.Context.Contracts;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class GenreService : IGenreService
    {
        private readonly IGenreRepo _genreRepo;
        private readonly IUnitOfWork _unitOfWork;

        public GenreService(IGenreRepo genreRepo, IUnitOfWork unitOfWork)
        {
            _genreRepo = genreRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GenreVM>> GetGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreRepo.GetGenresAsync(cancellationToken);

            return genres.Select(e => e.Map());
        }

        public async Task<GenreVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreRepo.GetByIdAsync(id, cancellationToken);

            return genre.Map();
        }

        public async Task<GenreVM> InsertAsync(GenreVM genreVM, CancellationToken cancellationToken)
        {
            var genre = genreVM.Map();

            _genreRepo.Insert(genre);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return genre.Map();
        }

        public async Task<GenreVM> UpdateAsync(GenreVM genreVM, CancellationToken cancellationToken)
        {
            var genre = genreVM.Map();

            _genreRepo.Update(genre);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return genre.Map();
        }

        public async Task<GenreVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await GetByIdAsync(id, cancellationToken);

            _genreRepo.DeleteById(genre.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return genre;
        }
    }
}
