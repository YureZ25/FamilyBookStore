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

        public void Insert(Book book)
        {
            _dbContext.CreateCommand()
                .WithText(@"INSERT INTO Books (Title, Description, AuthorId, GenreId) 
                    VALUES (@title, @description, @authorId, @genreId); 
                    SET @id = SCOPE_IDENTITY();")
                .WithParameter("id", book.Id, ParameterDirection.Output)
                .WithParameter("title", book.Title)
                .WithParameter("description", book.Description)
                .WithParameter("authorId", book.AuthorId)
                .WithParameter("genreId", book.GenreId);
        }

        public void Update(Book book)
        {
            _dbContext.CreateCommand()
                .WithText(@"UPDATE Books 
                    SET Title = @title, Description = @description, AuthorId = @authorId, GenreId = @genreId 
                    WHERE Id = @id")
                .WithParameter("id", book.Id)
                .WithParameter("title", book.Title)
                .WithParameter("description", book.Description)
                .WithParameter("authorId", book.AuthorId)
                .WithParameter("genreId", book.GenreId);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE Books WHERE Id = @id")
                .WithParameter("id", id);
        }

        private static Book Map(DbDataReader reader)
        {
            return new Book
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
                Store = new Store
                {
                    Id = reader.GetInt32(nameof(Book2Stores.StoreId)),
                    Name = reader.GetString("StoreName"),
                    Address = reader.GetString(nameof(Store.Address)),
                },
            };
        }
    }
}
