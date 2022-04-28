using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using DNVGL.Common.Core.JsonOptions;
using DNVGL.OAuth.Api.HttpClient;
using DNVGL.OAuth.Api.HttpClient.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNVGL.Veracity.Services.Api.Extensions
{
    public static class ConfigurationExtensions
    {
		public static IServiceCollection AddSerializer(this IServiceCollection services, Action<JsonSerializerOptions>? optionsSetup = null)
		{
			if (optionsSetup == null)
				services.AddWebDefaultJsonOptions();
			else
				services.AddOptions().Configure(optionsSetup);

	        services.TryAddTransient<ISerializer, JsonSerializer>();

            return services;
        }


        public static IServiceCollection AddApiV3<TInterface>(this IServiceCollection services, OAuthHttpClientOptions option, Func<IServiceProvider, TInterface> implementationFactory)
        {
            if (null == implementationFactory)
                throw new ArgumentNullException("implementationFactory");

            services.AddSerializer();
            services.AddOAuthHttpClient(option);

            services.AddSingleton(typeof(TInterface), sp=> implementationFactory.Invoke(sp));
            return services;
        }


        public static IServiceCollection AddApiV3OauthClientOptions(this IServiceCollection services, IEnumerable<OAuthHttpClientOptions> options)
        {
            services.AddOptions<ApiV3OAuthHttpClientOptions>()
                .Configure(o =>
                {
                    o.Options = options;
                });           

            return services;
        }


        public static OAuthHttpClientOptions GetApiV3OauthClientOption(this IServiceCollection services, string name)
        {
            var apiV3OauthClientOptions = services.BuildServiceProvider().GetRequiredService<IOptions<ApiV3OAuthHttpClientOptions>>().Value;

            var options = apiV3OauthClientOptions.Options.Where(x => x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

            if (options == null)
                throw new System.ArgumentException($"{name} not exist!");

            return options;
        }
    }
}
