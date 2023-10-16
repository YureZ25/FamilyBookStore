using Data.Context;
using Data.Entities;
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

        public async Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"SELECT 
                        Books.Id, 
                        Books.Title, 
                        Books.Description, 
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
                    LEFT JOIN Stores ON Book2Stores.StoreId = Stores.Id");

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            var books = new List<Book>();
            while (await reader.ReadAsync(cancellationToken))
            {
                books.Add(Map(reader));
            }
            return books;
        }

        public async Task<Book> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"SELECT 
                        Books.Id, 
                        Books.Title, 
                        Books.Description, 
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
                    JOIN Book2Stores ON Books.Id = Book2Stores.BookId
                    JOIN Stores ON Book2Stores.StoreId = Stores.Id
                    WHERE Books.Id = @id")
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public void AttachToStore(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText(@"INSERT INTO Book2Stores (BookId, StoreId)
                    VALUES (@bookId, @storeId);")
                .WithParameter(e => e.Id, "bookId", ParameterDirection.InputOutput)
                .WithParameter(e => e.Store.Id, "storeId");
        }

        public void DetachFromStore(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText(@"DELETE Book2Stores
                    WHERE BookId = @bookId AND StoreId = @storeId")
                .WithParameter(e => e.Id, "bookId")
                .WithParameter(e => e.Store.Id, "storeId");
        }

        public void Insert(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText(@"INSERT INTO Books (Title, Description, AuthorId, GenreId) 
                    VALUES (@title, @description, @authorId, @genreId); 
                    SET @id = SCOPE_IDENTITY();")
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.Title)
                .WithParameter(e => e.Description)
                .WithParameter(e => e.AuthorId)
                .WithParameter(e => e.GenreId);
        }

        public void Update(Book book)
        {
            _dbContext.CreateCommand(book)
                .WithText(@"UPDATE Books 
                    SET Title = @title, Description = @description, AuthorId = @authorId, GenreId = @genreId 
                    WHERE Id = @id")
                .WithParameter(e => e.Id)
                .WithParameter(e => e.Title)
                .WithParameter(e => e.Description)
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
            var book = new Book
            {
                Id = reader.GetInt32(nameof(Book.Id)),
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
                    Name = reader.GetString(nameof(Genre.Name)),
                },
            };

            if (reader.IsDBNull(nameof(Book2Stores.StoreId))) return book;

            book.Store = new Store
            {
                Id = reader.GetInt32(nameof(Book2Stores.StoreId)),
                Name = reader.GetString("StoreName"),
                Address = reader.GetString(nameof(Store.Address)),
            };

            return book;
        }
    }
}
