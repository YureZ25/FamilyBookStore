using Data.Context;
using Data.Entities;
using Data.Extensions;
using Data.Repos.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Data.Common;

namespace Data.Repos
{
    internal class UserRepo : IUserRepo
    {
        private readonly AdoNetDbContext _dbContext;

        public UserRepo(AdoNetDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return FindByIdAsync(Convert.ToInt32(userId), cancellationToken);
        }

        public async Task<User> FindByIdAsync(int userId, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT * FROM Users WHERE Id = @id")
                .WithParameter("id", userId);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT * FROM Users WHERE NormalizedUserName = @normalizedUserName")
                .WithParameter("normalizedUserName", normalizedUserName);

            using var reader = await cmd.ExecuteReaderAsync(cancellationToken);

            if (reader.HasRows && await reader.ReadAsync(cancellationToken))
            {
                return Map(reader);
            }

            return null;
        }

        public async Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("SELECT IIF(PasswordHash IS NOT NULL, 1, 0) FROM Users WHERE Id = @id")
                .WithParameter("id", user.Id);

            return await cmd.ExecuteScalarAsync(cancellationToken) is true;
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(Convert.ToString(user.Id));
        }

        public async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            var foundUser = await FindByIdAsync(user.Id, cancellationToken);

            return foundUser?.UserName;
        }

        public async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            var foundUser = await FindByIdAsync(user.Id, cancellationToken);

            return foundUser?.NormalizedUserName;
        }

        public async Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            var foundUser = await FindByIdAsync(user.Id, cancellationToken);

            return foundUser?.PasswordHash;
        }

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            var foundUser = await FindByIdAsync(user.Id, cancellationToken);

            foundUser.UserName = userName;

            await UpdateAsync(foundUser, cancellationToken);
        }

        public async Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            var foundUser = await FindByIdAsync(user.Id, cancellationToken);

            foundUser.NormalizedUserName = normalizedName;

            await UpdateAsync(foundUser, cancellationToken);
        }

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            var foundUser = await FindByIdAsync(user.Id, cancellationToken);

            foundUser.PasswordHash = passwordHash;

            await UpdateAsync(foundUser, cancellationToken);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"INSERT INTO Users (UserName, NormalizedUserName, PasswordHash)
                    VALUES (@userName, @normalizedUserName, @passwordHash);
                    SET @id = SCOPE_IDENTITY();")
                .WithParameter("id", user.Id, ParameterDirection.Output)
                .WithParameter("userName", user.UserName)
                .WithParameter("normalizedUserName", user.NormalizedUserName)
                .WithParameter("passwordHash", user.PasswordHash);

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText(@"UPDATE Users
                    SET UserName = @userName, NormalizedUserName = @normalizedUserName, PasswordHash = @passwordHash
                    WHERE Id = @id")
                .WithParameter("id", user.Id)
                .WithParameter("userName", user.UserName)
                .WithParameter("normalizedUserName", user.NormalizedUserName)
                .WithParameter("passwordHash", user.PasswordHash);

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("DELETE Users WHERE Id = @id")
                .WithParameter("id", user.Id);

            await cmd.ExecuteNonQueryAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }

        private static User Map(DbDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(nameof(User.Id)),
                UserName = reader.GetString(nameof(User.UserName)),
                NormalizedUserName = reader.GetString(nameof(User.NormalizedUserName)),
                PasswordHash = reader.GetString(nameof(User.PasswordHash)),
            };
        }
    }
}
