using Data.Context.Contracts;
using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;

namespace Data.Context
{
    internal class AdoNetDbContext : IDisposable
    {
        private bool _disposed;

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public Dictionary<IAdoNetCommand, IEntity> Commands { get; private set; } = [];

        public AdoNetDbContext(IConfiguration configuration)
        {
            var connectionStr = configuration.GetConnectionString("SqlServer");
            _connection = new SqlConnection(connectionStr);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public AdoNetQueryBuilder<T> CreateQuery<T>(Func<DbDataReader, T> mapFunc)
            where T : class, IEntity
        {
            var query = _connection.CreateCommand();
            query.Transaction = _transaction;
            return new(query, mapFunc);
        }

        public AdoNetScalarQueryBuilder<T> CreateScalarQuery<T>()
            where T : struct
        {
            var query = _connection.CreateCommand();
            query.Transaction = _transaction;
            return new(query);
        }

        public AdoNetCommandBuilder<T> CreateCommand<T>(T target)
            where T : class, IEntity
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            return new(this, command, target);
        }

        public async Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            int updated = 0;
            foreach (var (command, target) in Commands)
            {
                Commands.Remove(command);

                if (target is not null) command.ApplyNavigationsUpdates(target);

                updated += await command.ExecuteNonQuery(cancellationToken);

                if (target is null) continue;

                command.ApplyEntityUpdates(target);

                foreach (var otherCommandBuilder in Commands.Keys.Where(e => e.EntityType == command.EntityType))
                {
                    otherCommandBuilder.ApplyParametersUpdates(target);
                }
            }
            _transaction.Commit();
            Commands.Clear();
            return updated;
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (_transaction.Connection != null) _transaction.Rollback();
            _transaction.Dispose();

            if (_connection.State == ConnectionState.Open) _connection.Close();
            _connection.Dispose();

            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }
}
