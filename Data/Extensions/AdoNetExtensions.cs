using Microsoft.Data.SqlClient;
using System.Data;

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
    }
}
