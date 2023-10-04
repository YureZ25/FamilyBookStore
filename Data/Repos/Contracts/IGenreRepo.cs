using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IGenreRepo
    {
        Task<IEnumerable<Genre>> GetGenresAsync(CancellationToken cancellationToken);
        Task<Genre> GetByIdAsync(int id, CancellationToken cancellationToken);
        void Insert(Genre genre);
        void Update(Genre genre);
        void DeleteById(int id);
    }
}
