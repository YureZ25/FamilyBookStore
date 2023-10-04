using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IStoreRepo
    {
        Task<IEnumerable<Store>> GetStoresAsync(CancellationToken cancellationToken);
        Task<IEnumerable<Store>> GetStoresByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<Store> GetByIdAsync(int id, CancellationToken cancellationToken);
        void Insert(Store store);
        void Update(Store store);
        void DeleteById(int id);
    }
}
