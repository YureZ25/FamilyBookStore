﻿using Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Services.Services;
using Services.Services.Contracts;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddIdentityCore<User>().AddClaimsPrincipalFactory<AuthService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IGenreService, GenreService>();
            services.AddScoped<IBookQuoteService, BookQuoteService>();

            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ISearchService, SearchService>();

            return services;
        }
    }
}
