using Data.Entities.Contracts;

namespace Data.Repos.Contracts
{
    public interface IBaseRepo<T> where T : ICommonEntity
    {
        Task<IEnumerable<T>> GetAll(CancellationToken cancellationToken);
        Task<T> GetById(int id, CancellationToken cancellationToken);
        void Insert(T entity);
        void Update(T entity);
        void DeleteById(int id);
    }
}
