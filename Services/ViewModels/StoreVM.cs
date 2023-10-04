using Data.Entities;

namespace Services.ViewModels
{
    public class StoreVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public IEnumerable<BookVM> Books { get; set; }
    }

    internal static class StoreMapper
    {
        public static StoreVM Map(this Store store)
        {
            return new StoreVM
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Books = store.Books.Select(b => b.Map()),
            };
        }

        public static Store Map(this StoreVM storeVM)
        {
            return new Store
            {
                Id = storeVM.Id,
                Name = storeVM.Name,
                Address = storeVM.Address,
                Books = storeVM.Books.Select(b => b.Map()),
            };
        }
    }
}
