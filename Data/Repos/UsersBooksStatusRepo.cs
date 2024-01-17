using Data.Context;
using Data.Entities;
using Data.Enums;
using Data.Extensions;
using Data.Repos.Contracts;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class UsersBooksStatusRepo : IUsersBooksStatusRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public UsersBooksStatusRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private const string selectAllSql = """
            SELECT
                UsersBooksStatuses.Id,
                UsersBooksStatuses.BookStatus,
                UsersBooksStatuses.WishRead,
                UsersBooksStatuses.StartRead,
                UsersBooksStatuses.CurrentPage,
                UsersBooksStatuses.EndRead,
                UsersBooksStatuses.UserId,
                UsersBooksStatuses.BookId
            FROM UsersBooksStatuses
            """;

        public async Task<IEnumerable<UsersBooksStatus>> GetAll(CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText(selectAllSql);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            List<UsersBooksStatus> statuses = [];
            while (await reader.ReadAsync(cancellationToken))
            {
                statuses.Add(Map(reader));
            }
            return statuses;
        }

        public async Task<UsersBooksStatus> GetById(int id, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText($"""
                {selectAllSql}
                WHERE UsersBooksStatuses.Id = @id
                """)
                .WithParameter("id", id);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public async Task<UsersBooksStatus> GetStatus(int userId, int bookId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateQuery()
                .WithText($"""
                {selectAllSql}
                WHERE UsersBooksStatuses.UserId = @userId
                    AND UsersBooksStatuses.BookId = @bookId
                """)
                .WithParameter("userId", userId)
                .WithParameter("bookId", bookId);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public void Insert(UsersBooksStatus usersBooksStatus)
        {
            _dbContext.CreateCommand(usersBooksStatus)
                .WithText("""
                INSERT INTO UsersBooksStatuses (BookStatus, WishRead, StartRead, CurrentPage, EndRead, UserId, BookId)
                VALUES (@bookStatus, @wishRead, @startRead, @currentPage, @endRead, @userId, @bookId);
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.BookStatus)
                .WithParameter(e => e.WishRead)
                .WithParameter(e => e.StartRead)
                .WithParameter(e => e.CurrentPage)
                .WithParameter(e => e.EndRead)
                .WithParameter(e => e.UserId)
                .WithParameter(e => e.BookId);
        }

        public void Update(UsersBooksStatus usersBooksStatus)
        {
            _dbContext.CreateCommand(usersBooksStatus)
                .WithText("""
                UPDATE UsersBooksStatuses
                SET
                    BookStatus = @bookStatus,
                    WishRead = @wishRead,
                    StartRead = @startRead,
                    CurrentPage = @currentPage,
                    EndRead = @endRead,
                    UserId = @userId,
                    BookId = @bookId
                WHERE UsersBooksStatuses.Id = @id
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.BookStatus)
                .WithParameter(e => e.WishRead)
                .WithParameter(e => e.StartRead)
                .WithParameter(e => e.CurrentPage)
                .WithParameter(e => e.EndRead)
                .WithParameter(e => e.UserId)
                .WithParameter(e => e.BookId);
        }

        public void DeleteById(int id)
        {
            _dbContext.CreateCommand()
                .WithText("DELETE UsersBooksStatuses WHERE Id = @id")
                .WithParameter("id", id);
        }

        private UsersBooksStatus Map(DbDataReader reader)
        {
            return new UsersBooksStatus
            {
                Id = reader.Map<int>(nameof(UsersBooksStatus.Id)),
                BookStatus = (BookStatus)reader.Map<byte>(nameof(UsersBooksStatus.BookStatus)),
                WishRead = reader.Map<DateTime?>(nameof(UsersBooksStatus.WishRead)),
                StartRead = reader.Map<DateTime?>(nameof(UsersBooksStatus.StartRead)),
                CurrentPage = reader.Map<short?>(nameof(UsersBooksStatus.CurrentPage)),
                EndRead = reader.Map<DateTime?>(nameof(UsersBooksStatus.EndRead)),
                BookId = reader.Map<int>(nameof(UsersBooksStatus.BookId)),
                UserId = reader.Map<int>(nameof(UsersBooksStatus.UserId)),
            };
        }
    }
}
