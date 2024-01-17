using Data.Context.Contracts;
using Data.Entities;
using Data.Repos.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Services.Exeptions;
using Services.Services.Contracts;
using Services.ViewModels.StoreVMs;
using System.Security.Claims;

namespace Services.Services
{
    internal class StoreService : IStoreService
    {
        private readonly IStoreRepo _storeRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public StoreService(
            IStoreRepo storeRepo, 
            IUnitOfWork unitOfWork, 
            UserManager<User> userManager,
            IHttpContextAccessor contextAccessor)
        {
            _storeRepo = storeRepo;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<IEnumerable<StoreGetVM>> GetUserStoresOverview(int userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(Convert.ToString(userId))
                ?? throw new EntityNotFoundExeption("Пользователь", userId);

            var stores = await _storeRepo.GetStoresByUserId(user.Id, cancellationToken);

            var storesVM = stores.Take(3).Select(s =>
            {
                var storeVM = s.Map();
                storeVM.Books = storeVM.Books.Take(3);
                return storeVM;
            });

            return storesVM;
        }

        public async Task<IEnumerable<StoreGetVM>> GetStores(CancellationToken cancellationToken)
        {
            var stores = await _storeRepo.GetAll(cancellationToken);

            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId) ?? throw new EntityNotFoundExeption("Пользователь", userId);

            return stores.Select(s => s.Map(user));
        }

        public async Task<StoreGetVM> GetById(int id, CancellationToken cancellationToken)
        {
            var store = await _storeRepo.GetById(id, cancellationToken);

            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId) ?? throw new EntityNotFoundExeption("Пользователь", userId);

            return store.Map(user);
        }

        public async Task LinkStoreToUser(int storeId, CancellationToken cancellationToken)
        {
            var store = await _storeRepo.GetById(storeId, cancellationToken) ?? throw new EntityNotFoundExeption("Хранилище", storeId);

            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId) ?? throw new EntityNotFoundExeption("Пользователь", userId);

            _storeRepo.LinkStoreToUser(store, user);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public async Task UnlinkStoreFromUser(int storeId, CancellationToken cancellationToken)
        {
            var store = await _storeRepo.GetById(storeId, cancellationToken) ?? throw new EntityNotFoundExeption("Хранилище", storeId);

            var userId = _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId) ?? throw new EntityNotFoundExeption("Пользователь", userId);

            _storeRepo.UnlinkStoreFromUser(store, user);

            await _unitOfWork.SaveChanges(cancellationToken);
        }

        public async Task<StoreGetVM> Insert(StorePostVM storeVM, CancellationToken cancellationToken)
        {
            var store = storeVM.Map();

            _storeRepo.Insert(store);

            await _unitOfWork.SaveChanges(cancellationToken);

            return store.Map();
        }

        public async Task<StoreGetVM> Update(StorePostVM storeVM, CancellationToken cancellationToken)
        {
            var store = storeVM.Map();

            _storeRepo.Update(store);

            await _unitOfWork.SaveChanges(cancellationToken);

            return store.Map();
        }

        public async Task<StoreGetVM> DeleteById(int id, CancellationToken cancellationToken)
        {
            var store = await _storeRepo.GetById(id, cancellationToken);

            _storeRepo.DeleteById(store.Id);

            await _unitOfWork.SaveChanges(cancellationToken);

            return store.Map();
        }
    }
}
