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

        public async Task<IEnumerable<Store>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT 
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
                    Genres.Name AS GenreName,
                    Users2Stores.UserId,
                    Users.UserName,
                    Users.NormalizedUserName,
                    Users.PasswordHash
                FROM Stores
                LEFT JOIN Users2Stores ON Stores.Id = Users2Stores.StoreId
                LEFT JOIN Users ON Users2Stores.UserId = Users.Id
                LEFT JOIN Book2Stores ON Stores.Id = Book2Stores.StoreId
                LEFT JOIN Books ON Book2Stores.BookId = Books.Id
                LEFT JOIN Authors ON Books.AuthorId = Authors.Id
                LEFT JOIN Genres ON Books.GenreId = Genres.Id
                ORDER BY Stores.Id
                """)
                .Build();

            return await cmd.ToList(cancellationToken);
        }

        public async Task<IEnumerable<Store>> GetStoresByUserId(int userId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT 
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
                    Genres.Name AS GenreName,
                    Users2Stores.UserId,
                    Users.UserName,
                    Users.NormalizedUserName,
                    Users.PasswordHash
                FROM Stores
                JOIN Book2Stores ON Stores.Id = Book2Stores.StoreId
                JOIN Books ON Book2Stores.BookId = Books.Id
                JOIN Authors ON Books.AuthorId = Authors.Id
                JOIN Genres ON Books.GenreId = Genres.Id
                JOIN Users2Stores ON Stores.Id = Users2Stores.StoreId
                JOIN Users ON Users2Stores.UserId = Users.Id
                WHERE Users2Stores.UserId = @userId
                ORDER BY Stores.Id
                """)
                .WithParameter("userId", userId)
                .Build();

            return await cmd.ToList(cancellationToken);
        }

        public async Task<Store> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT 
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
                    Genres.Name AS GenreName,
                    Users2Stores.UserId,
                    Users.UserName,
                    Users.NormalizedUserName,
                    Users.PasswordHash
                FROM Stores
                LEFT JOIN Users2Stores ON Stores.Id = Users2Stores.StoreId
                LEFT JOIN Users ON Users2Stores.UserId = Users.Id
                LEFT JOIN Book2Stores ON Stores.Id = Book2Stores.StoreId
                LEFT JOIN Books ON Book2Stores.BookId = Books.Id
                LEFT JOIN Authors ON Books.AuthorId = Authors.Id
                LEFT JOIN Genres ON Books.GenreId = Genres.Id
                WHERE Stores.Id = @id
                """)
                .WithParameter(e => e.Id, id)
                .Build();

            return await cmd.FirstOrDefault(cancellationToken);
        }

        public void LinkStoreToUser(Store store, User user)
        {
            var u2s = new Users2Stores
            {
                StoreId = store.Id,
                UserId = user.Id,
            };

            _dbContext.CreateCommand(u2s)
                .WithText("""
                INSERT INTO Users2Stores (StoreId, UserId)
                VALUES (@storeId, @userId);
                """)
                .WithParameter(e => e.StoreId)
                .WithParameter(e => e.UserId)
                .Build();
        }

        public void UnlinkStoreFromUser(Store store, User user)
        {
            var u2s = new Users2Stores
            {
                StoreId = store.Id,
                UserId = user.Id,
            };

            _dbContext.CreateCommand(u2s)
                .WithText("""
                DELETE Users2Stores
                WHERE StoreId = @storeId AND UserId = @userId;
                """)
                .WithParameter(e => e.StoreId)
                .WithParameter(e => e.UserId)
                .Build();
        }

        public void Insert(Store store)
        {
            _dbContext.CreateCommand(store)
                .WithText("""
                INSERT INTO Stores (Name, Address) 
                VALUES (@name, @address); 
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.Name)
                .WithParameter(e => e.Address)
                .Build();
        }

        public void Update(Store store)
        {
            _dbContext.CreateCommand(store)
                .WithText("""
                UPDATE Stores 
                SET 
                    Name = @name, 
                    Address = @address 
                WHERE Id = @id;
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.Name)
                .WithParameter(e => e.Address)
                .Build();
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand<Store>(null)
                .WithText("DELETE Stores WHERE Id = @id")
                .WithParameter(e => e.Id, id)
                .Build();
        }

        private static Store Map(DbDataReader reader)
        {
            var store = new Store
            {
                Id = reader.Map<int>(nameof(Store.Id)),
                Name = reader.Map<string>(nameof(Store.Name)),
                Address = reader.Map<string>(nameof(Store.Address)),
            };

            if (!reader.IsDBNull(nameof(Book2Stores.BookId)))
            {
                var book = new Book
                {
                    Id = reader.Map<int>(nameof(Book2Stores.BookId)),
                    Title = reader.Map<string>(nameof(Book.Title)),
                    Description = reader.Map<string>(nameof(Book.Description)),
                    AuthorId = reader.Map<int>(nameof(Book.AuthorId)),
                    Author = new Author
                    {
                        Id = reader.Map<int>(nameof(Book.AuthorId)),
                        FirstName = reader.Map<string>(nameof(Author.FirstName)),
                        LastName = reader.Map<string>(nameof(Author.LastName)),
                    },
                    GenreId = reader.Map<int>(nameof(Book.GenreId)),
                    Genre = new Genre
                    {
                        Id = reader.Map<int>(nameof(Book.GenreId)),
                        Name = reader.Map<string>("GenreName"),
                    },
                    Store = store,
                };

                store.Books = [book];
            }

            if (!reader.IsDBNull(nameof(Users2Stores.UserId)))
            {
                var user = new User
                {
                    Id = reader.Map<int>(nameof(Users2Stores.UserId)),
                    UserName = reader.Map<string>(nameof(User.UserName)),
                    NormalizedUserName = reader.Map<string>(nameof(User.NormalizedUserName)),
                    PasswordHash = reader.Map<string>(nameof(User.PasswordHash)),
                };

                store.Users = [user];
            }

            return store;
        }
    }
}
