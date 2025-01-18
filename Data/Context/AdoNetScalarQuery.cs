using Microsoft.Data.SqlClient;

namespace Data.Context
{
    internal class AdoNetScalarQuery<T>
        where T : struct
    {
        private readonly SqlCommand _command;

        public AdoNetScalarQuery(SqlCommand command)
        {
            _command = command;
        }

        public async Task<T> Execute(CancellationToken cancellationToken)
        {
            var resObj = await _command.ExecuteScalarAsync(cancellationToken);

            return resObj is T res ? res : default;
        }
    }
}
