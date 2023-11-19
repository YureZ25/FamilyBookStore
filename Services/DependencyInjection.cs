using Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Contracts;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddIdentityCore<User>().AddClaimsPrincipalFactory<AuthService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IGenreService, GenreService>();

            return services;
        }
    }
}
