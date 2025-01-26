using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class BookQuoteRepo : IBookQuoteRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public BookQuoteRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BookQuote>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT
                    BookQuotes.Id,
                    BookQuotes.BookId,
                    BookQuotes.Text,
                    BookQuotes.Page
                FROM BookQuotes
                """)
                .Build();

            return await cmd.ToList(cancellationToken);
        }

        public async Task<IEnumerable<BookQuote>> GetByBookId(int bookId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT
                    BookQuotes.Id,
                    BookQuotes.BookId,
                    BookQuotes.Text,
                    BookQuotes.Page
                FROM BookQuotes
                WHERE BookQuotes.BookId = @bookId
                """)
                .WithParameter(e => e.BookId, bookId)
                .Build();

            return await cmd.ToList(cancellationToken);
        }

        public async Task<BookQuote> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery(Map)
                .WithText("""
                SELECT
                    BookQuotes.Id,
                    BookQuotes.BookId,
                    BookQuotes.Text,
                    BookQuotes.Page
                FROM BookQuotes
                WHERE BookQuotes.Id = @id
                """)
                .WithParameter(e => e.Id, id)
                .Build();

            return await cmd.FirstOrDefault(cancellationToken);
        }

        public void Insert(BookQuote entity)
        {
            _dbContext.CreateCommand(entity)
                .WithText("""
                INSERT INTO BookQuotes (BookId, Text, Page)
                VALUES (@bookId, @text, @page)
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.BookId)
                .WithParameter(e => e.Text)
                .WithParameter(e => e.Page)
                .Build();
        }

        public void Update(BookQuote entity)
        {
            _dbContext.CreateCommand(entity)
                .WithText("""
                UPDATE BookQuotes
                SET 
                    Text = @text, 
                    Page = @page
                WHERE Id = @id
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.Text)
                .WithParameter(e => e.Page)
                .Build();
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand<BookQuote>(null)
                .WithText("DELETE BookQuotes WHERE Id = @id")
                .WithParameter(e => e.Id, id)
                .Build();
        }

        private BookQuote Map(DbDataReader reader)
        {
            return new BookQuote
            {
                Id = reader.Map<int>(nameof(BookQuote.Id)),
                BookId = reader.Map<int>(nameof(BookQuote.BookId)),
                Text = reader.Map<string>(nameof(BookQuote.Text)),
                Page = reader.Map<short>(nameof(BookQuote.Page)),
            };
        }
    }
}
