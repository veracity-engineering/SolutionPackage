using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using DNVGL.OAuth.Api.HttpClient.TokenCache;
using Microsoft.Extensions.Caching.Distributed;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, ICollection<OAuthHttpClientFactoryOptions> options)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IOAuthHttpClientFactory>(s => new OAuthHttpClientFactory(options, s.GetService<IHttpContextAccessor>(), s.GetRequiredService<IMsalTokenCacheProvider>()));
            return services;
        }

        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, Action<ICollection<OAuthHttpClientFactoryOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientFactoryOptions>();
            configureOptions(options);
            return AddOAuthHttpClientFactory(services, options);
        }

        public static IServiceCollection AddDistributedTokenCache(this IServiceCollection services, Action<DistributedCacheEntryOptions> configureOptions = null)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60) };
            configureOptions?.Invoke(cacheEntryOptions);

            services.AddSingleton<IMsalTokenCacheProvider>(f => new MsalTokenCacheProvider(f.GetRequiredService<IDistributedCache>(), cacheEntryOptions));
            return services;
        }
    }
}
