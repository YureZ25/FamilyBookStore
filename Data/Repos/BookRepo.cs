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
                .WithText("SELECT * FROM Books");

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
                .WithText("SELECT * FROM Books WHERE Id = @id")
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
                .WithText("INSERT INTO Books (Title) VALUES (@title); SET @id = SCOPE_IDENTITY()")
                .WithParameter("id", book.Id, ParameterDirection.Output)
                .WithParameter("title", book.Title);
        }

        public void Update(Book book)
        {
            _dbContext.CreateCommand()
                .WithText("UPDATE Books SET Title = @title WHERE Id = @id")
                .WithParameter("id", book.Id)
                .WithParameter("title", book.Title);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE Books WHERE Id = @id")
                .WithParameter("id", id);
        }

        private Book Map(DbDataReader reader)
        {
            return new Book
            {
                Id = reader.GetInt32(nameof(Book.Id)),
                Title = reader.GetString(nameof(Book.Title)),
            };
        }
    }
}
