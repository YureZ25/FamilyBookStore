using Data.Repos.Contracts;
using Services.Services.Contracts;
using Services.ViewModels;

namespace Services.Services
{
    internal class StoreService : IStoreService
    {
        private readonly IBookRepo _bookRepo;

        public StoreService(IBookRepo bookRepo)
        {
            _bookRepo = bookRepo;
        }

        public async Task<IEnumerable<StoreVM>> GetUserStoresOverviewAsync(int userId, CancellationToken cancellationToken)
        {
            var books = await _bookRepo.GetBooksAsync(cancellationToken);
            var bookVMs = books.Select(e => e.Map());

            var stores = new StoreVM[]
            {
                new()
                {
                    Id = 0,
                    Name = "Дом Питер",
                    Books = bookVMs,
                },
                new()
                {
                    Id = 1,
                    Name = "E-Book Юры",
                    Books = bookVMs.Concat(bookVMs),
                },
                new()
                {
                    Id = 2,
                    Name = "E-Book Тани",
                    Books = bookVMs.Take(4),
                },
                new()
                {
                    Id = 3,
                    Name = "Донецк Патриотческая",
                    Books = bookVMs.Take(2),
                },
                new()
                {
                    Id = 4,
                    Name = "Донецк Шмидта",
                    Books = bookVMs,
                },
            };

            foreach (var store in stores)
            {
                store.Books = store.Books.Take(6);
            }

            return stores;
        }
    }
}
