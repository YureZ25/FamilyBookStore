using Data.Entities;

namespace Data.Repos.Contracts
{
    public interface IStoreRepo : IBaseRepo<Store>
    {
        Task<IEnumerable<Store>> GetStoresByUserId(int userId, CancellationToken cancellationToken);
        void LinkStoreToUser(Store store, User user);
        void UnlinkStoreFromUser(Store store, User user);
    }
}
