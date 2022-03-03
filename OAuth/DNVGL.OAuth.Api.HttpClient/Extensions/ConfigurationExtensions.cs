﻿using System;
using System.Collections.Generic;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
    public static class ConfigurationExtensions
    {
	    /// <summary>
        /// Add <see cref="OAuthHttpClientFactory"/> to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OAuthHttpClientFactory"/> instance to.</param>
        /// <param name="configureOptions">A method to manipulate the default options.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, Action<ICollection<OAuthHttpClientFactoryOptions>> configureOptions)
        {
            var options = new List<OAuthHttpClientFactoryOptions>();
            configureOptions(options);
            return AddOAuthHttpClientFactory(services, options);
        }

		/// <summary>
		/// Add <see cref="OAuthHttpClientFactory"/> to the specified <see cref="IServiceCollection"/>.
		/// </summary>
		/// <param name="services">The <see cref="IServiceCollection"/> to add the <see cref="OAuthHttpClientFactory"/> instance to.</param>
		/// <param name="options">A collection of configurations for the HttpClients produced by the factory.</param>
		/// <param name="cacheSetupAction"></param>
		/// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
		/// <returns></returns>
		public static IServiceCollection AddOAuthHttpClientFactory(this IServiceCollection services, IEnumerable<OAuthHttpClientFactoryOptions> options, Action<DistributedCacheEntryOptions>? cacheSetupAction = null)
	    {
		    services.AddHttpContextAccessor();

			services.AddOAuthCore(cacheSetupAction)
				.AddSingleton(s => 
				    new OAuthHttpClientFactory(options, 
					    s.GetRequiredService<IHttpContextAccessor>(), 
					    s.GetRequiredService<IClientAppBuilder>()))
			    .AddSingleton<IOAuthHttpClientFactory>(sp => sp.GetRequiredService<OAuthHttpClientFactory>())
			    .AddSingleton<IDelegatingHandlerBuilder>(sp => sp.GetRequiredService<OAuthHttpClientFactory>());

		    return services;
	    }
    }
}
