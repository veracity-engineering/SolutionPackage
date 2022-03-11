using System;
using System.Net.Http;
using DNV.Context.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Newtonsoft.Json;

namespace DNV.Context.HttpClient
{
    public static class HttpClientExtensions
    {
	    public static IServiceCollection AddHttpClientContext<T>(this IServiceCollection services, Action<IServiceProvider, System.Net.Http.HttpClient>? configAction = null, JsonSerializerSettings? jsonSerializerSettings = null) where T : class
	    {
		    var builder = configAction == null 
			    ? services.AddHttpClient(HttpClientContextHandler<T>.ClientName) 
			    : services.AddHttpClient(HttpClientContextHandler<T>.ClientName, configAction);

		    builder.AddContextHandler<T>(jsonSerializerSettings);

		    return services;
	    }

		public static IHttpClientBuilder AddContextHandler<T>(this IHttpClientBuilder builder, JsonSerializerSettings? jsonSerializerSettings = null) where T : class
		{
			builder.Services.AddTransient(sp =>
				new HttpClientContextHandler<T>(sp.GetRequiredService<IContextAccessor<T>>(), jsonSerializerSettings));

			builder.AddHttpMessageHandler<HttpClientContextHandler<T>>();

			return builder;
		}

		public static System.Net.Http.HttpClient CreateContextClient<T>(this IHttpClientFactory factory) where T : class
		{
			return factory.CreateClient(HttpClientContextHandler<T>.ClientName);
		}
	}
}
