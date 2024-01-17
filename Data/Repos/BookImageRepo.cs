using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class BookImageRepo : IBookImageRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public BookImageRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<BookImage>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT
                    BookImages.Id,
                    BookImages.FileName,
                    BookImages.ContentType,
                    BookImages.Content
                FROM BookImages
                """);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            List<BookImage> bookImages = [];
            while (await reader.ReadAsync(cancellationToken))
            {
                bookImages.Add(Map(reader));
            }
            return bookImages;
        }

        public async Task<BookImage> GetByBookId(int bookId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT
                    BookImages.Id,
                    BookImages.FileName,
                    BookImages.ContentType,
                    BookImages.Content
                FROM Books
                INNER JOIN BookImages ON Books.ImageId = BookImages.Id
                WHERE Books.Id = @bookId
                """)
                .WithParameter("bookId", bookId);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public async Task<BookImage> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT
                    BookImages.Id,
                    BookImages.FileName,
                    BookImages.ContentType,
                    BookImages.Content
                FROM BookImages
                WHERE BookImages.Id = @id
                """)
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public void Insert(BookImage bookImage)
        {
            _dbContext.CreateCommand(bookImage)
                .WithText("""
                INSERT INTO BookImages (FileName, ContentType, Content)
                VALUES (@fileName, @contentType, @content);
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.FileName)
                .WithParameter(e => e.ContentType)
                .WithParameter(e => e.Content);
        }

        public void Update(BookImage bookImage)
        {
            _dbContext.CreateCommand(bookImage)
                .WithText("""
                UPDATE BookImages
                SET
                    FileName = @fileName,
                    ContentType = @contentType,
                    Content = @content
                WHERE BookImages.Id = @id
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.FileName)
                .WithParameter(e => e.ContentType)
                .WithParameter(e => e.Content);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE BookImages WHERE Id = @id")
                .WithParameter("id", id);
        }

        private BookImage Map(DbDataReader reader)
        {
            return new BookImage
            {
                Id = reader.Map<int>(nameof(BookImage.Id)),
                FileName = reader.Map<string>(nameof(BookImage.FileName)),
                ContentType = reader.Map<string>(nameof(BookImage.ContentType)),
                Content = reader.Map<byte[]>(nameof(BookImage.Content)),
            };
        }
    }
}
