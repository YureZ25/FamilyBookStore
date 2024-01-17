using Data.Context.Contracts;
using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;
using Services.ViewModels.GenreVMs;

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

        public async Task<IEnumerable<GenreGetVM>> GetGenresAsync(CancellationToken cancellationToken)
        {
            var genres = await _genreRepo.GetAll(cancellationToken);

            return genres.Select(e => e.Map());
        }

        public async Task<GenreGetVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreRepo.GetById(id, cancellationToken);

            return genre.Map();
        }

        public async Task<GenreGetVM> InsertAsync(GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            var genre = genreVM.Map();

            _genreRepo.Insert(genre);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return genre.Map();
        }

        public async Task<GenreGetVM> UpdateAsync(GenrePostVM genreVM, CancellationToken cancellationToken)
        {
            var genre = genreVM.Map();

            _genreRepo.Update(genre);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return genre.Map();
        }

        public async Task<GenreGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreRepo.GetById(id, cancellationToken);

            _genreRepo.DeleteById(genre.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return genre.Map();
        }
    }
}
