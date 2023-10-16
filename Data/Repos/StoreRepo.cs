using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class StoreRepo : IStoreRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public StoreRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Store>> GetStoresAsync(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"SELECT 
                        Stores.Id, 
                        Stores.Name, 
                        Stores.Address, 
                        Book2Stores.BookId, 
                        Books.Title, 
                        Books.Description, 
                        Books.AuthorId, 
                        Authors.FirstName, 
                        Authors.LastName, 
                        Books.GenreId, 
                        Genres.Name AS GenreName
                    FROM Stores
                    LEFT JOIN Book2Stores ON Stores.Id = Book2Stores.StoreId
                    LEFT JOIN Books ON Book2Stores.BookId = Books.Id
                    LEFT JOIN Authors ON Books.AuthorId = Authors.Id
                    LEFT JOIN Genres ON Books.GenreId = Genres.Id
                    ORDER BY Stores.Id");

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var stores = new List<Store>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var tempStore = Map(reader);

                var store = stores.Find(e => e.Id == tempStore.Id);
                if (store != null)
                {
                    store.Books = store.Books.Append(tempStore.Books.Single());
                }
                else
                {
                    stores.Add(tempStore);
                }
            }
            return stores;
        }

        public async Task<IEnumerable<Store>> GetStoresByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"SELECT 
                        Stores.Id, 
                        Stores.Name, 
                        Stores.Address, 
                        Book2Stores.BookId, 
                        Books.Title, 
                        Books.Description, 
                        Books.AuthorId, 
                        Authors.FirstName, 
                        Authors.LastName, 
                        Books.GenreId, 
                        Genres.Name AS GenreName
                    FROM Stores
                    JOIN Book2Stores ON Stores.Id = Book2Stores.StoreId
                    JOIN Books ON Book2Stores.BookId = Books.Id
                    JOIN Authors ON Books.AuthorId = Authors.Id
                    JOIN Genres ON Books.GenreId = Genres.Id
                    JOIN Users2Stores ON Stores.Id = Users2Stores.StoreId
                    WHERE Users2Stores.UserId = @userId
                    ORDER BY Stores.Id")
                .WithParameter("userId", userId);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var stores = new List<Store>();
            while (await reader.ReadAsync(cancellationToken))
            {
                var tempStore = Map(reader);

                var store = stores.Find(e => e.Id == tempStore.Id);
                if (store != null)
                {
                    store.Books = store.Books.Append(tempStore.Books.Single());
                }
                else
                {
                    stores.Add(tempStore);
                }
            }
            return stores;
        }

        public async Task<Store> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"SELECT 
                        Stores.Id, 
                        Stores.Name, 
                        Stores.Address, 
                        Book2Stores.BookId, 
                        Books.Title, 
                        Books.Description, 
                        Books.AuthorId, 
                        Authors.FirstName, 
                        Authors.LastName, 
                        Books.GenreId, 
                        Genres.Name AS GenreName
                    FROM Stores
                    LEFT JOIN Book2Stores ON Stores.Id = Book2Stores.StoreId
                    LEFT JOIN Books ON Book2Stores.BookId = Books.Id
                    LEFT JOIN Authors ON Books.AuthorId = Authors.Id
                    LEFT JOIN Genres ON Books.GenreId = Genres.Id
                    WHERE Stores.Id = @id
                    ORDER BY Stores.Id")
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public void Insert(Store store)
        {
            _dbContext.CreateCommand(store)
                .WithText(@"INSERT INTO Stores (Name, Address) 
                    VALUES (@name, @address); 
                    SET @id = SCOPE_IDENTITY()")
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.Name)
                .WithParameter(e => e.Address);
        }

        public void Update(Store store)
        {
            _dbContext.CreateCommand(store)
                .WithText(@"UPDATE Stores 
                    SET Name = @name, Address = @address 
                    WHERE Id = @id")
                .WithParameter(e => e.Id)
                .WithParameter(e => e.Name)
                .WithParameter(e => e.Address);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE Stores WHERE Id = @id")
                .WithParameter("id", id);
        }

        private static Store Map(DbDataReader reader)
        {
            var store = new Store
            {
                Id = reader.GetInt32(nameof(Store.Id)),
                Name = reader.GetString(nameof(Store.Name)),
                Address = reader.GetString(nameof(Store.Address)),
            };

            if (reader.IsDBNull(nameof(Book2Stores.BookId))) return store;

            var book = new Book
            {
                Id = reader.GetInt32(nameof(Book2Stores.BookId)),
                Title = reader.GetString(nameof(Book.Title)),
                Description = reader.GetString(nameof(Book.Description)),
                AuthorId = reader.GetInt32(nameof(Book.AuthorId)),
                Author = new Author
                {
                    Id = reader.GetInt32(nameof(Book.AuthorId)),
                    FirstName = reader.GetString(nameof(Author.FirstName)),
                    LastName = reader.GetString(nameof(Author.LastName)),
                },
                GenreId = reader.GetInt32(nameof(Book.GenreId)),
                Genre = new Genre
                {
                    Id = reader.GetInt32(nameof(Book.GenreId)),
                    Name = reader.GetString("GenreName"),
                },
                Store = store,
            };

            store.Books = new[] { book };

            return store;
        }
    }
}
