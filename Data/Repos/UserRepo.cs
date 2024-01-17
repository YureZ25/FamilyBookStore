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
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT 
                    Users.Id,
                    Users.UserName,
                    Users.NormalizedUserName,
                    Users.PasswordHash
                FROM Users 
                WHERE Id = @id
                """)
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
            var cmd = _dbContext.CreateQuery()
                .WithText("""
                SELECT 
                    Users.Id,
                    Users.UserName,
                    Users.NormalizedUserName,
                    Users.PasswordHash
                FROM Users 
                WHERE Users.NormalizedUserName = @normalizedUserName
                """)
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
            var cmd = _dbContext.CreateQuery()
                .WithText("SELECT IIF(PasswordHash IS NOT NULL, 1, 0) FROM Users WHERE Id = @id")
                .WithParameter("id", user.Id);

            return await cmd.ExecuteScalarAsync(cancellationToken) is true;
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(Convert.ToString(user.Id));
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _dbContext.CreateCommand(user)
                .WithText("""
                INSERT INTO Users (UserName, NormalizedUserName, PasswordHash)
                VALUES (@userName, @normalizedUserName, @passwordHash);
                SET @id = SCOPE_IDENTITY();
                """)
                .WithParameter(e => e.Id, ParameterDirection.Output)
                .WithParameter(e => e.UserName)
                .WithParameter(e => e.NormalizedUserName)
                .WithParameter(e => e.PasswordHash);

            await _dbContext.SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            _dbContext.CreateCommand(user)
                .WithText("""
                UPDATE Users
                SET 
                    UserName = @userName, 
                    NormalizedUserName = @normalizedUserName, 
                    PasswordHash = @passwordHash
                WHERE Id = @id;
                """)
                .WithParameter(e => e.Id)
                .WithParameter(e => e.UserName)
                .WithParameter(e => e.NormalizedUserName)
                .WithParameter(e => e.PasswordHash);

            await _dbContext.SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var cmd = _dbContext.CreateCommand()
                .WithText("DELETE Users WHERE Id = @id")
                .WithParameter("id", user.Id);

            await _dbContext.SaveChanges(cancellationToken);

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
                Id = reader.Map<int>(nameof(User.Id)),
                UserName = reader.Map<string>(nameof(User.UserName)),
                NormalizedUserName = reader.Map<string>(nameof(User.NormalizedUserName)),
                PasswordHash = reader.Map<string>(nameof(User.PasswordHash)),
            };
        }
    }
}
