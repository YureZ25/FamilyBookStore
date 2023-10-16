using Data.Context.Contracts;
using Data.Entities.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Data.Context
{
    internal class AdoNetDbContext : IDisposable
    {
        private bool _disposed;

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public Dictionary<ICommandBuilder, IEntity> Commands { get; private set; } = new();

        public AdoNetDbContext(IConfiguration configuration)
        {
            var connectionStr = configuration.GetConnectionString("LocalSqlServer");
            _connection = new SqlConnection(connectionStr);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public SqlCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            Commands.Add(new AdoNetCommandBuilder<IEntity>(command, null), null);
            return command;
        }

        public AdoNetCommandBuilder<T> CreateCommand<T>(T target)
            where T : class, IEntity
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;

            var builder = new AdoNetCommandBuilder<T>(command, target);

            Commands.Add(builder, target);

            return builder;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int updated = 0;
            foreach (var (commandBuilder, target) in Commands)
            {
                updated += await commandBuilder.Command.ExecuteNonQueryAsync(cancellationToken);

                if (target is null) continue;

                commandBuilder.ApplyEntityUpdates(target);

                foreach (var otherCommandBuilder in Commands.Keys.Where(e => e.EntityType == commandBuilder.EntityType))
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
