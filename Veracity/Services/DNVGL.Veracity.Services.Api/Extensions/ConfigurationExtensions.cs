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

		//public static IServiceCollection AddApiResourceClientBuilder(this IServiceCollection services)
		//{
		//	return services.AddTransient<IApiResourceClientBuilder, ApiResourceClientBuilder>();			              
		//}
	}
}
