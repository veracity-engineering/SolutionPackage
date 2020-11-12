using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DNVGL.OAuth.Web.Abstractions;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Add <see cref="OAuthHttpClientFactory"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OAuthHttpClientFactory"/> instance to.</param>
        /// <param name="options">A collection of configurations for the HttpClients produced by the factory.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, IEnumerable<OAuthHttpClientFactoryOptions> options)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IOAuthHttpClientFactory>(s => new OAuthHttpClientFactory(options, s.GetService<IHttpContextAccessor>(), s.GetRequiredService<IClientAppBuilder>()));
            return services;
        }

        /// <summary>
        /// Add <see cref="OAuthHttpClientFactory"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OAuthHttpClientFactory"/> instance to.</param>
        /// <param name="configureOptions">A method to manipulate the default options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, Action<IEnumerable<OAuthHttpClientFactoryOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientFactoryOptions>();
            configureOptions(options);
            return AddOAuthHttpClientFactory(services, options);
        }
    }
}
