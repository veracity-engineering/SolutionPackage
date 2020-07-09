using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DNVGL.OAuth.UserCredentials
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, Action<ICollection<OAuthHttpClientFactoryOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientFactoryOptions>();
            configureOptions(options);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IOAuthHttpClientFactory>(s => new OAuthHttpClientFactory(options, s.GetService<IHttpContextAccessor>()));
            return services;
        }
    }
}
