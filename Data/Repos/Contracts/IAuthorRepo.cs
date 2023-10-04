using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IAuthorRepo
    {
        Task<IEnumerable<Author>> GetAuthorsAsync(CancellationToken cancellationToken);
        Task<Author> GetByIdAsync(int id, CancellationToken cancellationToken);
        void Insert(Author author);
        void Update(Author author);
        void DeleteById(int id);
    }
}
