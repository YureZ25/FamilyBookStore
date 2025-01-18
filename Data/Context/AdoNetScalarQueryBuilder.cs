using Microsoft.Data.SqlClient;
using System.Data;

namespace Data.Context
{
    internal class AdoNetScalarQueryBuilder<T> 
        where T : struct
    {
        private readonly SqlCommand _command;

        public AdoNetScalarQueryBuilder(SqlCommand command)
        {
            _command = command;
        }

        public AdoNetScalarQuery<T> Build()
        {
            return new(_command);
        }

        public AdoNetScalarQueryBuilder<T> WithText(string text)
        {
            _command.CommandText = text;
            return this;
        }

        public AdoNetScalarQueryBuilder<T> WithParameter(string parameter, object value)
        {
            _command.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.Input,
                ParameterName = parameter,
                Value = value,
            });
            return this;
        }
    }
}
