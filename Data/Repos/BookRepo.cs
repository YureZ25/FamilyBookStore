using Data.Context;
using Data.Entities;
using Data.Enums;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class BookRepo : IBookRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public BookRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        const string getBooksSql = """
            SELECT 
                Books.Id, 
                Books.Title, 
                Books.Description, 
                Books.IsbnStoreValue,
                Books.PageCount,
                Books.Price,
                Books.ImageId,
                Books.AuthorId, 
                Authors.FirstName, 
                Authors.LastName, 
                Books.GenreId, 
                Genres.Name, 
                Book2Stores.StoreId,
                Stores.Name AS StoreName,
                Stores.Address
            FROM Books 
            JOIN Authors ON Books.AuthorId = Authors.Id
            JOIN Genres ON Books.GenreId = Genres.Id
            LEFT JOIN Book2Stores ON Books.Id = Book2Stores.BookId
            LEFT JOIN Stores ON Book2Stores.StoreId = Stores.Id
            """;

        public async Task<IEnumerable<Book>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText(getBooksSql);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var books = new List<Book>();
            while (await reader.ReadAsync(cancellationToken))
            {
                books.Add(Map(reader));
            }
            return books;
        }

        public async Task<IEnumerable<Book>> GetBooksByUserStatus(int userId, BookStatus bookStatus, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT 
                    Books.Id, 
                    Books.Title, 
                    Books.Description, 
                    Books.IsbnStoreValue,
                    Books.PageCount,
                    Books.Price,
                    Books.ImageId,
                    Books.AuthorId, 
                    Authors.FirstName, 
                    Authors.LastName, 
                    Books.GenreId, 
                    Genres.Name, 
                    Book2Stores.StoreId,
                    Stores.Name AS StoreName,
                    Stores.Address,
                    UsersBooksStatuses.Id AS UsersBooksStatusesId,
                    UsersBooksStatuses.BookStatus,
                    UsersBooksStatuses.WishRead,
                    UsersBooksStatuses.StartRead,
                    UsersBooksStatuses.CurrentPage,
                    UsersBooksStatuses.EndRead,
                    UsersBooksStatuses.UserId
                FROM Books 
                JOIN Authors ON Books.AuthorId = Authors.Id
                JOIN Genres ON Books.GenreId = Genres.Id
                LEFT JOIN Book2Stores ON Books.Id = Book2Stores.BookId
                LEFT JOIN Stores ON Book2Stores.StoreId = Stores.Id
                LEFT JOIN UsersBooksStatuses ON UsersBooksStatuses.BookId = Books.Id
                WHERE UsersBooksStatuses.UserId = @userId AND UsersBooksStatuses.BookStatus = @bookStatus
                """)
                .WithParameter("userId", userId)
                .WithParameter("bookStatus", (byte)bookStatus);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var books = new List<Book>();
            while (await reader.ReadAsync(cancellationToken))
            {
                books.Add(Map(reader));
            }
            return books;
        }

        public async Task<IEnumerable<Book>> GetBooksByStore(int storeId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText($"""
                {getBooksSql}
                WHERE Stores.Id = @storeId
                """)
                .WithParameter("storeId", storeId);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var books = new List<Book>();
            while (await reader.ReadAsync(cancellationToken))
            {
                books.Add(Map(reader));
            }
            return books;
        }

        public async Task<Book> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText($"""
                {getBooksSql}
                WHERE Books.Id = @id
                """)
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public async Task<bool> AttachedToStore(int bookId, int storeId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT COUNT(*) FROM Book2Stores
                WHERE BookId = @bookId AND StoreId = @storeId
                """)
                .WithParameter(nameof(bookId), bookId)
                .WithParameter(nameof(storeId), storeId);

            return await cmd.ExecuteScalarAsync(cancellationToken) is > 0;
        }

        public void AttachToStore(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText("""
                INSERT INTO Book2Stores (BookId, StoreId)
                VALUES (@bookId, @storeId);
                """)
                .WithParameter(e => e.Id, "bookId", ParameterDirection.InputOutput)
                .WithParameter(e => e.Store.Id, "storeId");
        }

        public void DetachFromStore(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText("""
                DELETE Book2Stores
                WHERE BookId = @bookId
                """)
                .WithParameter(e => e.Id, "bookId");
        }

        public void Insert(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText("""
                INSERT INTO Books (Title, Description, IsbnStoreValue, PageCount, Price, ImageId, AuthorId, GenreId) 
                VALUES (@title, @description, @isbnStoreValue, @pageCount, @price, @imageId, @authorId, @genreId); 
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.Title)
                .WithParameter(e => e.Description)
                .WithParameter(e => e.IsbnStoreValue)
                .WithParameter(e => e.PageCount)
                .WithParameter(e => e.Price)
                .WithParameter(e => e.ImageId)
                .WithParameter(e => e.AuthorId)
                .WithParameter(e => e.GenreId);
        }

        public void Update(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText("""
                UPDATE Books 
                SET 
                    Title = @title, 
                    Description = @description, 
                    IsbnStoreValue = @isbnStoreValue, 
                    PageCount = @pageCount, 
                    Price = @price, 
                    ImageId = @imageId,
                    AuthorId = @authorId, 
                    GenreId = @genreId 
                WHERE Id = @id
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.Title)
                .WithParameter(e => e.Description)
                .WithParameter(e => e.IsbnStoreValue)
                .WithParameter(e => e.PageCount)
                .WithParameter(e => e.Price)
                .WithParameter(e => e.ImageId)
                .WithParameter(e => e.AuthorId)
                .WithParameter(e => e.GenreId);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE Books WHERE Id = @id")
                .WithParameter("id", id);
        }

        private static Book Map(DbDataReader reader)
        {
            var schema = reader.GetSchemaTable().AsEnumerable();

            var columnNames = schema.Select(r => r["ColumnName"]).Cast<string>();

            var book = new Book
            {
                Id = reader.Map<int>(nameof(Book.Id)),
                Title = reader.Map<string>(nameof(Book.Title)),
                Description = reader.Map<string>(nameof(Book.Description)),
                IsbnStoreValue = reader.Map<long?>(nameof(Book.IsbnStoreValue)),
                PageCount = reader.Map<short?>(nameof(Book.PageCount)),
                Price = reader.Map<decimal?>(nameof(Book.Price)),
                ImageId = reader.Map<int?>(nameof(Book.ImageId)),
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
                    Name = reader.Map<string>(nameof(Genre.Name)),
                },
            };

            if (columnNames.Contains(nameof(Book2Stores.StoreId)) && !reader.IsDBNull(nameof(Book2Stores.StoreId)))
            {
                book.Store = new Store
                {
                    Id = reader.Map<int>(nameof(Book2Stores.StoreId)),
                    Name = reader.Map<string>("StoreName"),
                    Address = reader.Map<string>(nameof(Store.Address)),
                };
            }

            if (columnNames.Contains("UsersBooksStatusesId") && !reader.IsDBNull("UsersBooksStatusesId"))
            {
                book.Status = new UsersBooksStatus
                {
                    Id = reader.Map<int>("UsersBooksStatusesId"),
                    BookStatus = (BookStatus)reader.Map<byte>(nameof(UsersBooksStatus.BookStatus)),
                    WishRead = reader.Map<DateTime?>(nameof(UsersBooksStatus.WishRead)),
                    StartRead = reader.Map<DateTime?>(nameof(UsersBooksStatus.StartRead)),
                    CurrentPage = reader.Map<short?>(nameof(UsersBooksStatus.CurrentPage)),
                    EndRead = reader.Map<DateTime?>(nameof(UsersBooksStatus.EndRead)),
                    UserId = reader.Map<int>(nameof(UsersBooksStatus.UserId)),
                    BookId = book.Id,
                    Book = book,
                };
            }

            return book;
        }
    }
}
