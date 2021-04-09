﻿using Microsoft.Extensions.DependencyInjection;

namespace DNVGL.Veracity.Services.Api.ApiV3.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddSerializer(this IServiceCollection services)
        {
            services.AddSingleton<ISerializer>(s => new JsonSerializer());
            return services;
        }
    }
}