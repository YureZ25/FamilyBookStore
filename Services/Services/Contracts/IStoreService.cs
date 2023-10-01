using Services.ViewModels;

namespace Services.Services.Contracts
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreVM>> GetUserStoresOverviewAsync(int userId, CancellationToken cancellationToken);
    }
}
