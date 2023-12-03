using Services.ViewModels.StoreVMs;

namespace Services.Services.Contracts
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreGetVM>> GetUserStoresOverviewAsync(int userId, CancellationToken cancellationToken);

        Task<IEnumerable<StoreGetVM>> GetStoresAsync(CancellationToken cancellationToken);
        Task<StoreGetVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task LinkStoreToUser(int storeId, CancellationToken cancellationToken);
        Task UnlinkStoreFromUser(int storeId, CancellationToken cancellationToken);
        Task<StoreGetVM> InsertAsync(StorePostVM storeVM, CancellationToken cancellationToken);
        Task<StoreGetVM> UpdateAsync(StorePostVM storeVM, CancellationToken cancellationToken);
        Task<StoreGetVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
