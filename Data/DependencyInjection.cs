using Data.Context;
using Data.Context.Contracts;
using Data.Repos;
using Data.Repos.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services)
        {
            services.AddScoped<AdoNetDbContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBookRepo, BookRepo>();

            return services;
        }
    }
}
