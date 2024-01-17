using Data.Context;
using Data.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Data
{
    public static class StartupTasks
    {
        public static async Task RunMigrateDbStartupTask(this IHost host, IHostEnvironment env)
        {
            using var scope = host.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AdoNetDbContext>();

            foreach (var scriptsDir in Directory.GetDirectories(Path.Combine(env.ContentRootPath, "sql-scripts")).OrderBy(x => x))
            {
                foreach (var filePath in Directory.GetFiles(scriptsDir).OrderBy(x => x))
                {
                    var sql = await File.ReadAllTextAsync(filePath);

                    context.CreateCommand().WithText(sql);
                }
            }

            await context.SaveChanges(CancellationToken.None);
        }
    }
}
