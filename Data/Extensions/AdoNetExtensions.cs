using System.Data;
using System.Data.Common;

namespace Data.Extensions
{
    internal static class AdoNetExtensions
    {
        private static readonly Dictionary<Type, Func<DbDataReader, string, dynamic>> _mappings = new() 
        {  
            [typeof(byte)] = (r, n) => r.GetByte(n),
            [typeof(short)] = (r, n) => r.GetInt16(n),
            [typeof(int)] = (r, n) => r.GetInt32(n),
            [typeof(long)] = (r, n) => r.GetInt64(n),
            [typeof(decimal)] = (r, n) => r.GetDecimal(n),
            [typeof(string)] = (r, n) => !r.IsDBNull(n) ? r.GetString(n) : null,
            [typeof(DateTime)] = (r, n) => !r.IsDBNull(n) ? r.GetDateTime(n) : null,
            [typeof(byte[])] = (r, n) =>
            {
                if (r.IsDBNull(n)) return null;

                byte[] buffer = new byte[4096];
                using MemoryStream bufferStream = new();

                long bytesRead = 0;
                long bytesReadTotal = 0;

                do
                {
                    bytesRead = r.GetBytes(n, bytesReadTotal, buffer, 0, buffer.Length);
                    bufferStream.Write(buffer, 0, (int)bytesRead);
                    bytesReadTotal += bytesRead;
                }
                while (bytesRead > 0);

                return bufferStream.ToArray();
            },
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
