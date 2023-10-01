using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Contracts;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IStoreService, StoreService>();

            return services;
        }
    }
}
