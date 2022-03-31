using System;
using System.Net.Http;
using System.Text.Json;
using DNV.Context.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace DNV.Context.HttpClient
{
    public static class HttpClientExtensions
    {
	    public static IServiceCollection AddHttpClientContext<T>(this IServiceCollection services, Action<IServiceProvider, System.Net.Http.HttpClient>? configAction = null, JsonSerializerOptions? jsonSerializerOptions = null) where T : class
	    {
		    var builder = configAction == null 
			    ? services.AddHttpClient(HttpClientContextHandler<T>.ClientName) 
			    : services.AddHttpClient(HttpClientContextHandler<T>.ClientName, configAction);

		    builder.AddHttpClientContextHandler<T>(jsonSerializerOptions);

		    return services;
	    }

		public static IHttpClientBuilder AddHttpClientContextHandler<T>(this IHttpClientBuilder builder, JsonSerializerOptions? jsonSerializerOptions = null) where T : class
		{
			builder.Services.TryAddTransient(sp =>
				new HttpClientContextHandler<T>(sp.GetRequiredService<IContextAccessor<T>>(), jsonSerializerOptions));

			builder.AddHttpMessageHandler<HttpClientContextHandler<T>>();

			return builder;
		}

		public static System.Net.Http.HttpClient CreateContextClient<T>(this IHttpClientFactory factory) where T : class
		{
			return factory.CreateClient(HttpClientContextHandler<T>.ClientName);
		}
	}
}
