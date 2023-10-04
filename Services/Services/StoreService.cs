using Data.Entities;
using Data.Repos.Contracts;
using Microsoft.AspNetCore.Identity;
using Services.Exeptions;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class StoreService : IStoreService
    {
        private readonly IStoreRepo _storeRepo;
        private readonly UserManager<User> _userManager;

        public StoreService(IStoreRepo storeRepo, UserManager<User> userManager)
        {
            _storeRepo = storeRepo;
            _userManager = userManager;
        }

        public async Task<IEnumerable<StoreVM>> GetUserStoresOverviewAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(Convert.ToString(userId))
                ?? throw new EntityNotFoundExeption("Пользователь", userId);

            var stores = await _storeRepo.GetStoresByUserIdAsync(user.Id, cancellationToken);

            var storesVM = stores.Select(s =>
            {
                var storeVM = s.Map();
                storeVM.Books = storeVM.Books.Take(6);
                return storeVM;
            });

            return storesVM;
        }
    }
}
