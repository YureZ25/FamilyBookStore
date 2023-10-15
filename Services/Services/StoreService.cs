using Data.Context.Contracts;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public StoreService(IStoreRepo storeRepo, IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _storeRepo = storeRepo;
            _unitOfWork = unitOfWork;
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

        public async Task<IEnumerable<StoreVM>> GetStoresAsync(CancellationToken cancellationToken)
        {
            var stores = await _storeRepo.GetStoresAsync(cancellationToken);

            return stores.Select(s => s.Map());
        }

        public async Task<StoreVM> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var store = await _storeRepo.GetByIdAsync(id, cancellationToken);

            return store.Map();
        }

        public async Task<StoreVM> InsertAsync(StoreVM storeVM, CancellationToken cancellationToken)
        {
            var store = storeVM.Map();

            _storeRepo.Insert(store);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return store.Map();
        }

        public async Task<StoreVM> UpdateAsync(StoreVM storeVM, CancellationToken cancellationToken)
        {
            var store = storeVM.Map();

            _storeRepo.Update(store);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return store.Map();
        }

        public async Task<StoreVM> DeleteByIdAsync(int id, CancellationToken cancellationToken)
        {
            var store = await GetByIdAsync(id, cancellationToken);

            _storeRepo.DeleteById(store.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return store;
        }
    }
}
