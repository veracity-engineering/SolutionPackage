using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
    public static class OAuthHttpClientFactoryExtensions
    {
        /// <summary>
        /// Add <see cref="OAuthHttpClientFactory"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OAuthHttpClientFactory"/> instance to.</param>
        /// <param name="configureOptions">A method to manipulate the default options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, Action<ICollection<OAuthHttpClientOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientOptions>();
            configureOptions(options);

            return services.AddOAuthHttpClientFactory(options);
        }

        /// <summary>
        /// Add <see cref="OAuthHttpClientFactory"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OAuthHttpClientFactory"/> instance to.</param>
        /// <param name="options">A collection of configurations for the HttpClients produced by the factory.</param>
        /// <param name="cacheSetupAction"></param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        /// <returns></returns>
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, IEnumerable<OAuthHttpClientOptions> options, Action<DistributedCacheEntryOptions>? cacheSetupAction = null)
        {
            services.AddOAuthHttpClients(options, cacheConfigAction: cacheSetupAction);

            return services;
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name">Value to match <see cref="OAuthHttpClientOptions.Name">Name</see> of configuration options for the created instance.</param>
        /// <param name="configOverride">an Action which allow to override some configs for the client configuration.</param>
        /// <returns></returns>
        public static System.Net.Http.HttpClient CreateWithUserCredentialFlow(this IOAuthHttpClientFactory factory, string name, Action<OAuthHttpClientOptions>? configOverride = null)
        {
            return factory.Create(c => c.Name == name 
                                       && c.Flow == OAuthCredentialFlow.UserCredentials, configOverride);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="name">Value to match <see cref="OAuthHttpClientOptions.Name">Name</see> of configuration options for the created instance.</param>
        /// <param name="configOverride">an Action which allow to override some configs for the client configuration.</param>
        /// <returns></returns>
        public static System.Net.Http.HttpClient CreateWithClientCredentialFlow(this IOAuthHttpClientFactory factory, string name, Action<OAuthHttpClientOptions>? configOverride = null)
        {
            return factory.Create(c => c.Name == name
                                       && c.Flow == OAuthCredentialFlow.ClientCredentials, configOverride);
        }
    }
}
