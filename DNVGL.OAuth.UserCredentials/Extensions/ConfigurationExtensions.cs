using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DNVGL.OAuth.Web.Abstractions;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, IEnumerable<OAuthHttpClientFactoryOptions> options)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IOAuthHttpClientFactory>(s => new OAuthHttpClientFactory(options, s.GetService<IHttpContextAccessor>(), s.GetRequiredService<IMsalAppBuilder>()));
            return services;
        }

        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, Action<ICollection<OAuthHttpClientFactoryOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientFactoryOptions>();
            configureOptions(options);
            return AddOAuthHttpClientFactory(services, options);
        }
    }
}
