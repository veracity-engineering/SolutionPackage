using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNVGL.Veracity.Services.Api.Extensions
{
    public static class ConfigurationExtensions
    {
	    internal static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web);

		public static IServiceCollection AddSerializer(this IServiceCollection services, Action<JsonSerializerOptions>? optionsSetup = null)
        {
	        if (optionsSetup == null)
				services.TryAddSingleton(_ => Options.Create(DefaultJsonSerializerOptions));
			else
				services.AddOptions().Configure(optionsSetup);

	        services.TryAddTransient<ISerializer, JsonSerializer>();

            return services;
        }

		internal static void CopyTo(this JsonSerializerOptions source, JsonSerializerOptions target)
		{
			target.AllowTrailingCommas = source.AllowTrailingCommas;
			target.IgnoreNullValues = source.IgnoreNullValues;
			target.IgnoreReadOnlyProperties = source.IgnoreReadOnlyProperties;
			target.DefaultBufferSize = source.DefaultBufferSize;
			target.MaxDepth = source.MaxDepth;
			target.DefaultIgnoreCondition = source.DefaultIgnoreCondition;
			target.DictionaryKeyPolicy = source.DictionaryKeyPolicy;
			target.Encoder = source.Encoder;
			target.IgnoreReadOnlyFields = source.IgnoreReadOnlyFields;
			target.IncludeFields = source.IncludeFields;
			target.NumberHandling = source.NumberHandling;
			target.PropertyNameCaseInsensitive = source.PropertyNameCaseInsensitive;
			target.ReadCommentHandling = source.ReadCommentHandling;
			target.ReferenceHandler = source.ReferenceHandler;
			target.WriteIndented = source.WriteIndented;
			target.PropertyNamingPolicy = source.PropertyNamingPolicy;
			target.Converters.Clear();
			source.Converters.ToList().ForEach(c => target.Converters.Add(c));
		}
    }
}
