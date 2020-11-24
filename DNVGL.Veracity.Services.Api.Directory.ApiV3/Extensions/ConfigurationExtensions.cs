﻿using DNVGL.OAuth.Api.HttpClient;
using DNVGL.Veracity.Services.Api.ApiV3;
using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.Directory.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddUserDirectory(this IServiceCollection services)
        {
            services.AddScoped<ISerializer>(s => new JsonSerializer());
            services.AddScoped<IUserDirectory>(s => new UserDirectory(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>()));
            return services;
        }

        public static IServiceCollection AddCompanyDirectory(this IServiceCollection services)
        {
            services.AddScoped<ISerializer>(s => new JsonSerializer());
            services.AddScoped<ICompanyDirectory>(s => new CompanyDirectory(s.GetRequiredService<IOAuthHttpClientFactory>(), s.GetRequiredService<ISerializer>()));
            return services;
        }
    }
}
