using Data.Entities;
using Services.ViewModels.BookVMs;

namespace Services.ViewModels.StoreVMs
{
    internal static class StoreMapper
    {
        public static StoreGetVM Map(this Store store)
        {
            return new StoreGetVM
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Books = store.Books?.Select(b => b.Map()),
            };
        }

        public static StoreGetVM Map(this Store store, User user)
        {
            return new StoreGetVM
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Books = store.Books?.Select(b => b.Map()),
                IsLinkedToUser = store.Users.Any(u => u.Id == user.Id),
            };
        }

        public static Store Map(this StorePostVM storeVM)
        {
            return new Store
            {
                Id = storeVM.Id ?? 0,
                Name = storeVM.Name,
                Address = storeVM.Address,
            };
        }
    }
}
