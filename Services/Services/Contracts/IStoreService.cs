using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreVM>> GetUserStoresOverviewAsync(int userId, CancellationToken cancellationToken);

        Task<IEnumerable<StoreVM>> GetStoresAsync(CancellationToken cancellationToken);
        Task<StoreVM> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<StoreVM> InsertAsync(StoreVM storeVM, CancellationToken cancellationToken);
        Task<StoreVM> UpdateAsync(StoreVM storeVM, CancellationToken cancellationToken);
        Task<StoreVM> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    }
}
