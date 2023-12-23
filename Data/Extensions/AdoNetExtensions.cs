using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Data.Extensions
{
    internal static class AdoNetExtensions
    {
        public static SqlCommand WithText(this SqlCommand command, string text)
        {
            command.CommandText = text;
            return command;
        }

        public static SqlCommand WithParameter(this SqlCommand command, string parameter, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            command.Parameters.Add(new SqlParameter
            {
                ParameterName = parameter,
                Direction = direction,
                Value = value,
            });
            return command;
        }

        private static Dictionary<Type, Func<DbDataReader, string, dynamic>> _mappings = new() 
        {  
            [typeof(int)] = (r, n) => r.GetInt32(n),
            [typeof(long)] = (r, n) => r.GetInt64(n),
            [typeof(decimal)] = (r, n) => r.GetDecimal(n),
            [typeof(string)] = (r, n) => !r.IsDBNull(n) ? r.GetString(n) : null,
        };

        public static T Map<T>(this DbDataReader reader, string columnName)
        {
            var type = Nullable.GetUnderlyingType(typeof(T));

            var isNullable = type is not null;

            type ??= typeof(T);

            if (!_mappings.TryGetValue(type, out var mapping))
            {
                throw new ArgumentException($"Mapping for type {typeof(T)} was not found");
            }

            if (isNullable && reader.IsDBNull(columnName))
            {
                return default;
            }

            return (T)mapping(reader, columnName);
        }
    }
}
