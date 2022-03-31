using System;
using System.Net.Http;
using System.Text.Json;
using DNV.Context.Abstractions;
using DNVGL.Common.Core.JsonOptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace DNV.Context.HttpClient
{
    public static class HttpClientExtensions
    {
	    public static IServiceCollection AddHttpClientContext<T>(this IServiceCollection services, Action<IServiceProvider, System.Net.Http.HttpClient>? configAction = null, Action<JsonSerializerOptions>? jsonOptionSetup = null) where T : class
	    {
		    var builder = configAction == null 
			    ? services.AddHttpClient(HttpClientContextHandler<T>.ClientName) 
			    : services.AddHttpClient(HttpClientContextHandler<T>.ClientName, configAction);

		    if (jsonOptionSetup == null)
			    services.AddWebDefaultJsonOptions();
		    else
			    services.AddOptions().Configure(jsonOptionSetup);

			builder.AddHttpClientContextHandler<T>();

		    return services;
	    }

		public static IHttpClientBuilder AddHttpClientContextHandler<T>(this IHttpClientBuilder builder) where T : class
		{
			builder.Services.TryAddTransient(sp =>
				new HttpClientContextHandler<T>(sp.GetRequiredService<IContextAccessor<T>>(), sp.GetService<IOptions<JsonSerializerOptions>>()));

			builder.AddHttpMessageHandler<HttpClientContextHandler<T>>();

			return builder;
		}

		public static System.Net.Http.HttpClient CreateContextClient<T>(this IHttpClientFactory factory) where T : class
		{
			return factory.CreateClient(HttpClientContextHandler<T>.ClientName);
		}
	}
}
