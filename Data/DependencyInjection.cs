using Data.Context;
using Data.Context.Contracts;
using Data.Entities;
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
            services.AddScoped<IAuthorRepo, AuthorRepo>();
            services.AddScoped<IGenreRepo, GenreRepo>();
            services.AddScoped<IStoreRepo, StoreRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IUsersBooksStatusRepo, UsersBooksStatusRepo>();

            services.AddIdentityCore<User>()
                .AddUserStore<UserRepo>();

            return services;
        }
    }
}
