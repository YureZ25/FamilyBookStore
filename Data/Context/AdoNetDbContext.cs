using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Data.Context
{
    internal class AdoNetDbContext : IDisposable
    {
        private bool _disposed;

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public IList<SqlCommand> Commands { get; private set; } = new List<SqlCommand>();

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
            Commands.Add(command);
            return command;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int updated = 0;
            foreach (var command in Commands)
            {
                updated += await command.ExecuteNonQueryAsync(cancellationToken);
            }
            _transaction.Commit();
            return updated;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _transaction.Rollback();
            _transaction.Dispose();

            _connection.Close();
            _connection.Dispose();

            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }
}
