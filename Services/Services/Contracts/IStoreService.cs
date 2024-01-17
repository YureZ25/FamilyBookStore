using Services.ViewModels.StoreVMs;

namespace Services.Services.Contracts
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreGetVM>> GetUserStoresOverview(int userId, CancellationToken cancellationToken);

        Task<IEnumerable<StoreGetVM>> GetStores(CancellationToken cancellationToken);
        Task<StoreGetVM> GetById(int id, CancellationToken cancellationToken);
        Task LinkStoreToUser(int storeId, CancellationToken cancellationToken);
        Task UnlinkStoreFromUser(int storeId, CancellationToken cancellationToken);
        Task<StoreGetVM> Insert(StorePostVM storeVM, CancellationToken cancellationToken);
        Task<StoreGetVM> Update(StorePostVM storeVM, CancellationToken cancellationToken);
        Task<StoreGetVM> DeleteById(int id, CancellationToken cancellationToken);
    }
}
