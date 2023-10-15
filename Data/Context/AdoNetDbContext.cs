using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;

namespace Data.Context
{
    internal class AdoNetDbContext : IDisposable
    {
        private bool _disposed;

        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;

        public Dictionary<SqlCommand, object> Commands { get; private set; } = new Dictionary<SqlCommand, object>();

        public AdoNetDbContext(IConfiguration configuration)
        {
            var connectionStr = configuration.GetConnectionString("LocalSqlServer");
            _connection = new SqlConnection(connectionStr);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public SqlCommand CreateCommand(object target = null)
        {
            var command = _connection.CreateCommand();
            command.Transaction = _transaction;
            Commands.Add(command, target);
            return command;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int updated = 0;
            foreach (var (command, target) in Commands)
            {
                updated += await command.ExecuteNonQueryAsync(cancellationToken);

                SetKeyProperty(command, target);
            }
            _transaction.Commit();
            Commands.Clear();
            return updated;
        }

        private void SetKeyProperty(SqlCommand command, object target)
        {
            if (target == null) return;

            var keyProp = target.GetType().GetProperties()
                .SingleOrDefault(e => e.GetCustomAttribute<KeyAttribute>() != null);

            if (keyProp == null) return;

            var keyParam = command.Parameters
                .Cast<SqlParameter>()
                .Where(p => p.Direction is ParameterDirection.Output or ParameterDirection.InputOutput)
                .FirstOrDefault(p => p.ParameterName.Equals(keyProp.Name, StringComparison.InvariantCultureIgnoreCase));

            if (keyParam == null) throw new ApplicationException($"Output param for property {keyProp} was not found. Param must have same name as prop.");

            keyProp.SetValue(target, keyParam.Value);
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
