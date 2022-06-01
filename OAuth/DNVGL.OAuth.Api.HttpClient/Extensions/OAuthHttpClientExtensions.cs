using System;
using System.Collections.Generic;
using System.Linq;
using DNV.OAuth.Abstractions;
using DNV.OAuth.Core;
using DNVGL.OAuth.Api.HttpClient.HttpClientHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DNVGL.OAuth.Api.HttpClient.Extensions
{
	/// <summary>
	/// 
	/// </summary>
    public static class OAuthHttpClientExtensions
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="optionsConfigAction"></param>
		/// <param name="configBuilderAction"></param>
		/// <param name="clientConfigAction"></param>
		/// <param name="cacheConfigAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddOAuthHttpClients(this IServiceCollection services,
			Action<ICollection<OAuthHttpClientOptions>> optionsConfigAction,
			Action<IHttpClientBuilder>? configBuilderAction = null,
			Action<IServiceProvider, System.Net.Http.HttpClient>? clientConfigAction = null,
			Action<DistributedCacheEntryOptions>? cacheConfigAction = null)
		{
			var options = new List<OAuthHttpClientOptions>();
			optionsConfigAction(options);

			return services.AddOAuthHttpClients(options, configBuilderAction, clientConfigAction, cacheConfigAction);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="options"></param>
		/// <param name="configBuilderAction"></param>
		/// <param name="clientConfigAction"></param>
		/// <param name="cacheConfigAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddOAuthHttpClients(this IServiceCollection services, 
			IEnumerable<OAuthHttpClientOptions> options, 
			Action<IHttpClientBuilder>? configBuilderAction = null, 
			Action<IServiceProvider, System.Net.Http.HttpClient>? clientConfigAction = null, 
			Action<DistributedCacheEntryOptions>? cacheConfigAction = null)
		{
			options.ToList().ForEach(o => services.AddOAuthHttpClient(o, configBuilderAction, clientConfigAction, cacheConfigAction));

			return services;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		/// <param name="option"></param>
		/// <param name="configBuilderAction"></param>
		/// <param name="clientConfigAction"></param>
		/// <param name="cacheConfigAction"></param>
		/// <returns></returns>
		public static IServiceCollection AddOAuthHttpClient(this IServiceCollection services, 
			OAuthHttpClientOptions option, 
			Action<IHttpClientBuilder>? configBuilderAction = null, 
			Action<IServiceProvider, System.Net.Http.HttpClient>? clientConfigAction = null, 
			Action<DistributedCacheEntryOptions>? cacheConfigAction = null)
		{
			services.Configure<OAuthHttpClientOptions>($"{option.Name}", o =>
			{//add this named option with "Name" for compatible with old apiv3 implmentation
				o.Bind(option);
			});

			services.Configure<OAuthHttpClientOptions>($"{option.Name}:{ option.Flow}", o=>
			{
				o.Bind(option);		
			});

			var builder = services.AddHttpClient($"{option.Name}:{option.Flow}", 
				clientConfigAction ?? ((_, clt) => clt.BaseAddress = new Uri(option.BaseUri)));

			builder.AddOAuthHttpClientHandler(option, cacheConfigAction);

			configBuilderAction?.Invoke(builder);

			return services;
		}

		public static OAuthHttpClientOptions GetOauthClientOptions(this IServiceProvider serviceProvider, string name)
		{
			var oauthClientOptions = serviceProvider.GetService<IOptionsMonitor<OAuthHttpClientOptions>>();

			var options = oauthClientOptions.Get(name);
			if (options == null
				|| string.IsNullOrEmpty(options.Name))
			{
				throw new System.ArgumentException($"Configuration: {name} does not exist!");
			}

			return options;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="options"></param>
		/// <param name="cacheConfigAction"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static IHttpClientBuilder AddOAuthHttpClientHandler(this IHttpClientBuilder builder, OAuthHttpClientOptions options, Action<DistributedCacheEntryOptions>? cacheConfigAction = null)
		{

			switch (options.Flow)
			{
				case OAuthCredentialFlow.UserCredentials:
					builder.AddHttpMessageHandler(sp =>
						new UserCredentialsHandler(
							options, 
							sp.GetRequiredService<IHttpContextAccessor>(), 
							sp.GetRequiredService<IClientAppBuilder>()));
					break;
				case OAuthCredentialFlow.ClientCredentials:
					builder.AddHttpMessageHandler(sp => 
						new ClientCredentialsHandler(
							options, 
							sp.GetRequiredService<IClientAppBuilder>()));
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(options.Flow));
			}

			builder.Services.AddMandatoryDependencies(cacheConfigAction);

			return builder;
		}

		private static IServiceCollection AddMandatoryDependencies(this IServiceCollection services,
			Action<DistributedCacheEntryOptions>? cacheConfigAction)
		{
			return services.AddHttpContextAccessor()
				.AddOAuthCore(cacheConfigAction);
		}
	}
}
