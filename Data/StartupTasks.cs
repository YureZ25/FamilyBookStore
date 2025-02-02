using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

namespace Data
{
    public static class StartupTasks
    {
        public static async Task RunMigrateDbStartupTask(this IHost host, IHostEnvironment env, IConfiguration configuration)
        {
            var connectionStr = configuration.GetConnectionString("SqlServer");
            var connection = new SqlConnection(connectionStr);

            try
            {
                connection.Open();
            }
            catch (SqlException ex) when (ex.ErrorCode == -2146232060)
            {
                CreateDbIfNotExist();

                connection = new SqlConnection(connectionStr);
                connection.Open();
            }

            var transaction = connection.BeginTransaction();

            foreach (var scriptsDir in Directory.GetDirectories(Path.Combine(env.ContentRootPath, "sql-scripts")).OrderBy(x => x))
            {
                foreach (var filePath in Directory.GetFiles(scriptsDir).OrderBy(x => x))
                {
                    var sql = await File.ReadAllTextAsync(filePath);
                    sql = Regex.Replace(sql, @"\$\((db-name)\)", connection.Database);

                    var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = sql;

                    await command.ExecuteNonQueryAsync();
                }
            }

            transaction.Commit();


            void CreateDbIfNotExist()
            {
                var initDbKey = "Initial Catalog";

                var connectionData = connectionStr.Split(";")
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(x => x.Split("="))
                    .ToDictionary(k => k[0], v => v[1]);

                var serverConnectionData = connectionData
                    .Where(e => e.Key != initDbKey)
                    .Select(e => $"{e.Key}={e.Value}");
                var serverConnectionStr = string.Join(';', serverConnectionData);

                var serverConnection = new SqlConnection(serverConnectionStr);
                try
                {
                    serverConnection.Open();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Failed connect to sql server while trying create new db", ex);
                }

                var command = serverConnection.CreateCommand();
                command.CommandText = $"""
                IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{connection.Database}')
                BEGIN
                    CREATE DATABASE [{connection.Database}];
                END;
                """;
                command.ExecuteNonQuery();
                serverConnection.Close();
            }
        }
    }
}
